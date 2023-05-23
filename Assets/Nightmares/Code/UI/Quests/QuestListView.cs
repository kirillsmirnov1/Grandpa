using Nightmares.Code.Model.Quests;
using UnityEngine;
using UnityUtils.Variables;

namespace Nightmares.Code.UI.Quests
{
    public class QuestListView : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private QuestManager quests;
        [SerializeField] private QuestEntryView entryPrefab;
        [SerializeField] private IntVariable maxUnlockedLevel;
        
        [Header("Components")]
        [SerializeField] private RectTransform questContainer;

        public void Show()
        {
            DestroyOldEntries(); // TODO reuse UU list/entry 
            SpawnEntries();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void DestroyOldEntries()
        {
            for (int i = questContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(questContainer.GetChild(i).gameObject);
            }
        }

        private void SpawnEntries()
        {
            var allQuests = quests.quests;
            foreach (var quest in allQuests)
            {
                Instantiate(entryPrefab, questContainer)
                    .Set(unlocked: quest.minLevel <= maxUnlockedLevel,
                        completed: quest.Complete,
                        txt: quest.displayName);
            }
        }
    }
}
