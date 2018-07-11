using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [CreateAssetMenu(fileName = "localize", menuName = "M8/Localize/From Google Sheet")]
    public class LocalizeAssetGoogleSheet : LocalizeAsset {
        [HideInInspector]
        public SpreadsheetParser.GoogleSettings googleSettings;
        [HideInInspector]
        public string googleSheetName;
    }
}