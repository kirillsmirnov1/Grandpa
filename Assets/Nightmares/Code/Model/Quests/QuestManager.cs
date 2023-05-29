using System.Linq;
using UnityEngine;

namespace Nightmares.Code.Model.Quests
{
    [CreateAssetMenu(fileName = "New Quest Manager", menuName = "Data/Quest Manager", order = 0)]
    public class QuestManager : ScriptableObject
    {
        /// <summary>
        /// All available quests
        /// </summary>
        [SerializeField] public Quest[] quests;

        private Quest[] _activeQuests;
        
        public void PrepareForSession()
        {
            foreach (var quest in quests)
            {
                quest.PrepareForSession();
            }

            _activeQuests = quests.Where(q => !q.Complete).ToArray();
        }

        public Quest[] CompletedInSession => _activeQuests.Where(q => q.Complete).ToArray();
    }
}