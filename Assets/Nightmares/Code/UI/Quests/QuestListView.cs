using System.Linq;
using Nightmares.Code.Model.Quests;
using UnityEngine;
using UnityUtils.Variables;
using UnityUtils.View;

namespace Nightmares.Code.UI.Quests
{
    [RequireComponent(typeof(PanelVisibilityAnimator))]
    public class QuestListView : ListView<QuestDisplayData>
    {
        [SerializeField] private PanelVisibilityAnimator panelVisibilityAnimator;

        [Header("Resources")]
        [SerializeField] private QuestManager quests;
        [SerializeField] private IntVariable maxUnlockedLevel;

        public void Show()
        {
            Show(quests.quests);
        }
        
        public void Show(Quest[] questsToShow)
        {
            gameObject.SetActive(true);

            var sortedQuests = questsToShow
                .OrderBy(q => q.IsCompleted)
                .ThenBy(q => q.minLevel);
            var displayData = sortedQuests
                .Select(q => q.ToDisplayData(maxUnlockedLevel))
                .ToList(); 
            
            SetEntries(displayData);

            panelVisibilityAnimator.Show();
        }

        public void Hide()
        {
            panelVisibilityAnimator.Hide();
        }
    }
}
