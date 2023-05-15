using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nightmares.Code.UI
{
    public class StartBanner : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        private Action _startCallback;
        
        public void Show(Action startCallback)
        {
            _startCallback = startCallback;
            gameObject.SetActive(true);
        }
        
        private void OnEnable()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void OnStartButtonClick()
        {
            _startCallback?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
