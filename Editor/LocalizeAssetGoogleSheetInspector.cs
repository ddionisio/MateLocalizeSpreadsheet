using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace M8 {
    [CustomEditor(typeof(LocalizeAssetGoogleSheet))]
    public class LocalizeAssetGoogleSheetInspector : Editor {
        public class Item {
            public string Key = "";
            public string Value = "";
            public string[] Params = new string[0];
        }

        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors) {
            //Debug.Log("Validation successful!");
            return true;
        }

        public override void OnInspectorGUI() {
            var dat = target as LocalizeAssetGoogleSheet;

            var googleSettings = (SpreadsheetParser.GoogleSettings)EditorGUILayout.ObjectField("Google Settings", dat.googleSettings, typeof(SpreadsheetParser.GoogleSettings), false);
            if(dat.googleSettings != googleSettings) {
                Undo.RecordObject(dat, "Changed Google Settings.");
                dat.googleSettings = googleSettings;
            }

            var spreadsheetName = EditorGUILayout.TextField("Spreadsheet", dat.googleSheetName);
            if(dat.googleSheetName != spreadsheetName) {
                Undo.RecordObject(dat, "Changed GoogleSheet Name.");
                dat.googleSheetName = spreadsheetName;
            }

            if(GUILayout.Button("Import"))
                Import();

            EditorGUILayout.Separator();

            base.OnInspectorGUI();
        }
                
        void OnEnable() {
            // resolve TlsException error
            ServicePointManager.ServerCertificateValidationCallback += Validator;
        }

        void OnDisable() {
            // resolve TlsException error
            ServicePointManager.ServerCertificateValidationCallback -= Validator;
        }

        private void Import() {
            var dat = target as LocalizeAssetGoogleSheet;

            var parser = new SpreadsheetParser.GoogleParser(dat.googleSettings, dat.googleSheetName);

            //refresh database
            string error = "";
            if(!parser.GenerateDatabase(ref error)) {
                EditorUtility.DisplayDialog("Error", error, "OK");
                return;
            }

            //deserialize
            var dataLookup = parser.DeserializeAllSheets<Item>(null);

            //fill in data
            var tableList = new List<LocalizeAsset.TableData>();

            foreach(var pair in dataLookup) {
                string language = pair.Key;
                List<Item> items = pair.Value;

                var table = new LocalizeAsset.TableData();

                //TODO: check if platform
                table.name = language;
                table.entryType = LocalizeAsset.TableEntryType.Language;

                //Populate entries
                var entryList = new List<LocalizeAsset.Entry>();

                foreach(var item in items)
                    entryList.Add(new LocalizeAsset.Entry() { key = item.Key, text = item.Value, param = item.Params });

                table.entries = entryList.ToArray();

                tableList.Add(table);
            }

            dat.tables = tableList.ToArray();

            EditorUtility.SetDirty(dat);
        }
    }
}