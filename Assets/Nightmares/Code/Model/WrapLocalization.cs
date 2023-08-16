using UnityEngine;
using UnityEngine.Localization.Tables;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Localization;
#endif

namespace Nightmares.Code.Model
{
    [CreateAssetMenu(fileName = "WrapLocalization", menuName = "Data/WrapLocalization", order = 0)]
    public class WrapLocalization : ScriptableObject
    {
#if UNITY_EDITOR
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
                    if(!kv.Key.Contains("story")) continue;
                    foreach (var s in stringsToWrap)
                    {
                        // TODO replace only first entry 
                        kv.Value = kv.Value.Replace(s, string.Format(wrap, s));
                    }
                }
            }
        }
#endif
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
                EditorUtility.SetDirty(target);
            }
        }
    }
    #endif
}
