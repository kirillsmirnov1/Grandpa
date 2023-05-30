using Nightmares.Code.Model.Quests;
using Nightmares.Code.UI.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nightmares.Code.UI
{
    public class GameOverBanner : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private Sprite winSprite;
        [SerializeField] private Sprite loseSprite;
        [SerializeField] private Color winColor;
        [SerializeField] private Color loseColor;
        
        [Header("Components")]
        [SerializeField] private Image mainImage;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI bodyTextScore;
        [SerializeField] private QuestListView questListView;
        [SerializeField] private TextMeshProUGUI buttonText;
        
        public void Set(bool victory, int score, int difficulty, Quest[] completedQuests)
        {
            mainImage.sprite = victory ? winSprite : loseSprite;
            mainImage.color = victory ? winColor : loseColor;
            
            headerText.text = victory ? "You won!" : "You lost!";
            bodyTextScore.text = $"score: {score}\n" +
                            $"difficulty: {difficulty}";

            var hasCompletedQuests = completedQuests != null && completedQuests.Length != 0; 
            questListView.gameObject.SetActive(hasCompletedQuests);

            if (hasCompletedQuests)
            {
                questListView.Show(completedQuests);
            }

            buttonText.text = "continue";
        }
    }
}
