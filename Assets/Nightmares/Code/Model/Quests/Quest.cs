using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nightmares.Code.Model.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Data/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] public string displayName;
        [SerializeField] public int minLevel;
        [SerializeField] private QuestTask[] tasks;

        [SerializeField, HideInInspector] private string guid;
        
        public bool IsCompleted { get; set; }
        public bool CanBeCompleted => tasks.All(t => t.Complete);
        public string UID => guid;

#if UNITY_EDITOR
        private void OnValidate()
        {
            CheckGuidSerialization();
        }

        private void CheckGuidSerialization()
        {
            guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(this)).ToString();
            EditorUtility.SetDirty(this);
        }
#endif

        public void PrepareForSession(bool isCompleted)
        {
            IsCompleted = isCompleted;
            
            foreach (var task in tasks)
            {
                task.PrepareForSession();
            }
        }
    }
}