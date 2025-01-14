using System;
using Nightmares.Code.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils.Variables;

namespace Nightmares.Code.UI
{
    public class StartBanner : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private float wallSpeed = 1;
        
        [Header("Prefabs")]
        [SerializeField] private TextMeshProUGUI numberPrefab;
        
        [Header("Components")] 
        [SerializeField] private RawImage[] walls;
        [SerializeField] private CenterScroll difficultyCenterScroll;
        [SerializeField] private RectTransform difficultyScrollContent;
        
        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        [Header("Data")]
        [SerializeField] private IntVariable maxUnlockedDifficulty;
        [SerializeField] private IntVariable currentDifficulty;
        
        private Action _startCallback;
        
        public void Show(Action startCallback)
        {
            _startCallback = startCallback;
            for (int i = 0; i < maxUnlockedDifficulty.Value; i++)
            {
                var letter = Instantiate(numberPrefab, difficultyScrollContent);
                letter.text = $"{i + 1}";
            }
            difficultyCenterScroll.enabled = true;
            gameObject.SetActive(true);
            this.DelayAction(() => difficultyCenterScroll.ScrollTo(currentDifficulty.Value - 1, true), 0);
        }
        
        private void OnEnable()
        {
            leftButton.onClick.AddListener(OnLeftButtonClick);
            rightButton.onClick.AddListener(OnRightButtonClick);
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        private void OnDisable()
        {
            leftButton.onClick.RemoveListener(OnLeftButtonClick);
            rightButton.onClick.RemoveListener(OnRightButtonClick);
            startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) OnRightButtonClick();
            if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) OnLeftButtonClick();

            var wallDist = Time.deltaTime * wallSpeed;
            foreach (var wall in walls)
            {
                var rect = wall.uvRect;
                rect.y += wallDist;
                wall.uvRect = rect;
            }
        }

        private void OnRightButtonClick() => ScrollDifficulty(1);
        private void OnLeftButtonClick() => ScrollDifficulty(-1);
        private void ScrollDifficulty(int difficultyChange)
        {
            difficultyCenterScroll.ScrollBy(difficultyChange);
        }

        private void OnStartButtonClick()
        {
            currentDifficulty.Value = difficultyCenterScroll.FindElementClosestToCenter() + 1;
            _startCallback?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
