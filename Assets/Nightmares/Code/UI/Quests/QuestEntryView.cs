using Nightmares.Code.Model.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils.View;

namespace Nightmares.Code.UI.Quests
{
    public class QuestEntryView : ListViewEntry<QuestDisplayData>
    {
        [Header("Sprites")]
        [SerializeField] private Sprite questComplete;
        [SerializeField] private Sprite questIncomplete;

        [Header("Components")]
        [SerializeField] private Image statusImage;
        [SerializeField] private TextMeshProUGUI questText;

        public override void Fill(QuestDisplayData data)
        {
            base.Fill(data);
            statusImage.sprite = data.IsCompleted ? questComplete : questIncomplete;
            questText.text = data.IsUnlocked ? data.Description : "????????????";
        }
    }
}
