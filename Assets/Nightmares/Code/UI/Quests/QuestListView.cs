using System.Linq;
using Nightmares.Code.Model.Quests;
using UnityEngine;
using UnityUtils.Variables;
using UnityUtils.View;

namespace Nightmares.Code.UI.Quests
{
    public class QuestListView : ListView<QuestDisplayData>
    {
        [Header("Resources")]
        [SerializeField] private QuestManager quests;
        [SerializeField] private IntVariable maxUnlockedLevel;
        
        public void Show()
        {
            gameObject.SetActive(true);
            var allQuests = quests.quests;
            SetEntries(allQuests.Select(ToDisplayData).ToList());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private QuestDisplayData ToDisplayData(Quest quest) => new() {
            Description = quest.displayName,
            IsUnlocked = quest.minLevel <= maxUnlockedLevel,
            IsCompleted = quest.Complete
        };
    }
}
