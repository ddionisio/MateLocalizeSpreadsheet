using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace M8 {
    [CustomEditor(typeof(LocalizeAssetExcel))]
    public class LocalizeAssetExcelInspector : Editor {
        public override void OnInspectorGUI() {
            var data = target as LocalizeAssetExcel;

            GUILayout.BeginHorizontal();
            GUILayout.Label("File:", GUILayout.Width(50));

            string path = string.Empty;
            if(string.IsNullOrEmpty(data.excelFilePath))
                path = Application.dataPath;
            else
                path = data.excelFilePath;

            EditorGUILayout.LabelField(path);
            if(GUILayout.Button("...", GUILayout.Width(25))) {
                string folder = Path.GetDirectoryName(path);
#if UNITY_EDITOR_WIN
                path = EditorUtility.OpenFilePanel("Open Excel file", folder, "excel files;*.xls;*.xlsx");
#else // for UNITY_EDITOR_OSX
                path = EditorUtility.OpenFilePanel("Open Excel file", folder, "xls");
#endif

                // the path should be relative not absolute one to make it work on any platform.
                int index = path.IndexOf("Assets");
                if(index >= 0) {
                    // set relative path
                    path = path.Substring(index);

                    if(path.Length > 0 && data.excelFilePath != path) {
                        Undo.RecordObject(data, "Changed Excel File Path.");

                        data.excelFilePath = path;
                    }
                }
                else {
                    EditorUtility.DisplayDialog("Error",
                        @"Wrong folder is selected.
                        Set a folder under the 'Assets' folder! \n
                        The excel file should be anywhere under  the 'Assets' folder", "OK");
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            if(GUILayout.Button("Import"))
                Import();
            
            EditorExt.Utility.DrawSeparator();

            base.OnInspectorGUI();
        }

        void Import() {
            var data = target as LocalizeAssetExcel;

            string path = data.excelFilePath;

            if(string.IsNullOrEmpty(path)) {
                string msg = "You should specify spreadsheet file first!";
                EditorUtility.DisplayDialog("Error", msg, "OK");
                return;
            }

            if(!File.Exists(path)) {
                string msg = string.Format("File at {0} does not exist.", path);
                EditorUtility.DisplayDialog("Error", msg, "OK");
                return;
            }

            string errorMsg = "";
            if(!LocalizeAssetExcelPostprocess.ApplyAsset(data, ref errorMsg)) {
                EditorUtility.DisplayDialog("Error", errorMsg, "OK");
                return;
            }

            EditorUtility.SetDirty(data);
        }
    }
}