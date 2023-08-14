using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nightmares.Code.Model
{
    [CreateAssetMenu(fileName = "WrapLocalization", menuName = "Data/WrapLocalization", order = 0)]
    public class WrapLocalization : ScriptableObject
    {
        public string wrap = "a{0}b";
        public string[] stringsToWrap;
        public LocalizationTableCollection table;
        
        public void Wrap()
        {
            // TODO 
            foreach (var subTable in table.Tables)
            {
                var asset = subTable.asset as StringTable;
                foreach (var kv in asset.Values)
                {
                    Debug.Log(kv.Value);
                    foreach (var s in stringsToWrap)
                    {
                        kv.Value = kv.Value.Replace(s, string.Format(wrap, s));
                    }
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(WrapLocalization))]
    public class WrapLocalizationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Wrap"))
            {
                (target as WrapLocalization).Wrap();
            }
        }
    }
#endif
}