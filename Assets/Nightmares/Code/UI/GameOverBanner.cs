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
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private TextMeshProUGUI buttonText;
        
        public void Set(bool victory, int score, int difficulty, string[] completedQuests)
        {
            mainImage.sprite = victory ? winSprite : loseSprite;
            mainImage.color = victory ? winColor : loseColor;
            
            headerText.text = victory ? "You won!" : "You lost!";
            bodyText.text = $"score: {score}\n" +
                            $"difficulty: {difficulty}";

            if (completedQuests != null && completedQuests.Length != 0)
                bodyText.text += $"\n× × ×\nEndured Torments:\n\n{string.Join("\n\n", completedQuests)}";

            buttonText.text = "continue";
        }
    }
}
