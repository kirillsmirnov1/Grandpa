using UnityEngine;
using UnityEngine.UI;

namespace Nightmares.Code.Model
{
    public class TouchControlsEnabler : MonoBehaviour
    {
        public static bool TouchControlsEnabled
            => _instance.forceMobileControls
               || Application.platform == RuntimePlatform.Android;
        
        private static TouchControlsEnabler _instance;
        
        public bool forceMobileControls = false;

        [SerializeField] private GameObject mobileCanvas;
        [SerializeField] private Button forceEnableMobileControlsButton;
        
        private void Awake()
        {
            _instance = this;
            EnableMobileCanvas();
        }

        private void Update()
        {
#if UNITY_EDITOR
            EnableMobileCanvas();
#endif
        }

        private void EnableMobileCanvas()
        {
#if UNITY_WEBGL
            forceEnableMobileControlsButton.gameObject.SetActive(!forceMobileControls);
            forceEnableMobileControlsButton.onClick.AddListener(OnForceEnableMobileControlsButtonClick);
#else
            forceEnableMobileControlsButton.gameObject.SetActive(false);
#endif
            mobileCanvas.SetActive(TouchControlsEnabled);
        }
#if UNITY_WEBGL
        private void OnForceEnableMobileControlsButtonClick()
        {
            forceMobileControls = true;
            forceEnableMobileControlsButton.gameObject.SetActive(false);
            mobileCanvas.SetActive(true);
        }
#endif
    }
}
