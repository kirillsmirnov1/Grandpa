using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nightmares.Code.UI
{
    public class StoryCardButton : MonoBehaviour
    {
        public ButtonType type;
        public int sceneToLoad;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            switch (type)
            {
                case ButtonType.Info:
                    break;
                case ButtonType.Tasks:
                    break;
                case ButtonType.Play:
                    SceneManager.LoadScene(sceneToLoad);
                    break;
                case ButtonType.Read:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum ButtonType
        {
            Info, 
            Tasks, 
            Play, 
            Read,
        }
    }
}
