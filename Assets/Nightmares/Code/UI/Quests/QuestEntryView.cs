using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nightmares.Code.UI.Quests
{
    public class QuestEntryView : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite questComplete;
        [SerializeField] private Sprite questIncomplete;

        [Header("Components")]
        [SerializeField] private Image statusImage;
        [SerializeField] private TextMeshProUGUI questText;

        public void Set(bool unlocked, bool completed, string txt)
        {
            statusImage.gameObject.SetActive(unlocked);
            statusImage.sprite = completed ? questComplete : questIncomplete;
            questText.text = unlocked ? txt : "???????????????";
        }
    }
}
