using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [CreateAssetMenu(fileName = "localize", menuName = "M8/Localize/From Excel")]
    public class LocalizeAssetExcel : LocalizeAsset {
        public string excelFilePath;
        
    }
}