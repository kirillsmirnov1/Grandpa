using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtils.Variables;

namespace Nightmares.Code.Model.Quests
{
    [CreateAssetMenu(fileName = "New Quest Manager", menuName = "Data/Quest Manager", order = 0)]
    public class QuestManager : ScriptableObject
    {
        /// <summary>
        /// All available quests
        /// </summary>
        [SerializeField] public Quest[] quests;
        [SerializeField] private StringArrayVariable completedQuestsSave;

        private Quest[] _activeQuests;
        private HashSet<string> _completedQuests;

        public void PrepareForSession()
        {
            InitSavedQuests();
            
            foreach (var quest in quests)
            {
                quest.PrepareForSession(_completedQuests.Contains(quest.UID));
            }

            _activeQuests = quests.Where(q => !q.IsCompleted).ToArray();
        }

        public void SaveCompletedQuests()
        {
            var newCompleted = CompletedInSession;
            if (newCompleted == null || newCompleted.Length == 0) return;
            
            foreach (var quest in newCompleted)
            {
                _completedQuests.Add(quest.UID);
            }

            completedQuestsSave.Value = _completedQuests.ToList();
        }

        private void InitSavedQuests()
        {
            if(_completedQuests != null) return;

            _completedQuests = new HashSet<string>(completedQuestsSave.Value);
        }

        public Quest[] CompletedInSession => _activeQuests.Where(q => q.CanBeCompleted).ToArray();
    }
}