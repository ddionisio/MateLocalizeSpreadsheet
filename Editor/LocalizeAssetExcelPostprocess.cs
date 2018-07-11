using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace M8 {
    public class LocalizeAssetExcelPostprocess : AssetPostprocessor {
        public const string headerKeyName = "Key";
        public const string headerValueName = "Value";
        public const string headerParamName = "Params";

        public class Item {
            public string Key = "";
            public string Value = "";
            public string[] Params = new string[0];
        }

        public static bool ApplyAsset(LocalizeAssetExcel localize, ref string error) {
            var excelParser = new SpreadsheetParser.ExcelParser(localize.excelFilePath);

            if(excelParser.sheetCount == 0) {
                error = "No sheets are found.";
                return false;
            }

            var tableList = new List<LocalizeAsset.TableData>();

            var dataLookup = excelParser.DeserializeAllSheets<Item>(null);

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

            localize.tables = tableList.ToArray();
            
            EditorUtility.SetDirty(localize);

            AssetDatabase.SaveAssets();
            
            return true;
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            LocalizeAssetExcel localizeAssetExcel = null;

            if(Localize.isInstantiated) {
                localizeAssetExcel = Localize.instance as LocalizeAssetExcel;
            }
            else {
                string path = Localize.assetPath;
                if(!string.IsNullOrEmpty(path))
                    localizeAssetExcel = AssetDatabase.LoadAssetAtPath<LocalizeAssetExcel>(path);
            }

            if(localizeAssetExcel) {
                for(int i = 0; i < importedAssets.Length; i++) {
                    if(importedAssets[i] == localizeAssetExcel.excelFilePath) {
                        string errorMsg = "";
                        if(!ApplyAsset(localizeAssetExcel, ref errorMsg))
                            Debug.LogWarning(string.Format("Error processing LocalizeAssetExcel ({0}): {1}", localizeAssetExcel.name, errorMsg));
                    }
                }
            }
        }
    }
}