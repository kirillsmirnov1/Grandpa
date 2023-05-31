using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Localization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nightmares.Code.Model.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Data/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private LocalizedString description;
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

        public QuestDisplayData ToDisplayData(int maxUnlockedLevel)
        {
            return new QuestDisplayData
            {
                Description = description.GetLocalizedString() + TasksDescription,
                IsUnlocked = minLevel <= maxUnlockedLevel,
                IsCompleted = IsCompleted
            };
        }

        private string TasksDescription
        {
            get
            {
                if (IsCompleted) return "";
                var str = new StringBuilder();
                foreach (var task in tasks)
                {
                    if(!task.ShowProgress) continue;
                    if (str.Length == 0) str.Append("\n");
                    str.Append(task.CurrentProgress + " ");
                }
                return str.ToString();
            }
        }
    }
}