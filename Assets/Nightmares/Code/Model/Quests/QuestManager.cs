using UnityEngine;

namespace Nightmares.Code.Model.Quests
{
    [CreateAssetMenu(fileName = "New Quest Manager", menuName = "Data/Quest Manager", order = 0)]
    public class QuestManager : ScriptableObject
    {
        /// <summary>
        /// All available quests
        /// </summary>
        [SerializeField] private Quest[] quests;

        public void PrepareForSession()
        {
            foreach (var quest in quests)
            {
                quest.PrepareForSession();
            }
        }
    }
}