using UnityEngine;
using UnityEngine.UI;

namespace Nightmares.Code.Model
{
    public class TouchControlsEnabler : MonoBehaviour
    {
        public static bool TouchControlsEnabled 
            => _instance.forceMobileControls 
               || Application.platform == RuntimePlatform.Android
               || Prefs.ForcedMobileControls;
        
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
            forceEnableMobileControlsButton.gameObject.SetActive(!Prefs.ForcedMobileControls);
            forceEnableMobileControlsButton.onClick.AddListener(OnForceEnableMobileControlsButtonClick);
#else
            forceEnableMobileControlsButton.gameObject.SetActive(false);
#endif
            mobileCanvas.SetActive(TouchControlsEnabled);
        }
#if UNITY_WEBGL
        private void OnForceEnableMobileControlsButtonClick()
        {
            Prefs.ForcedMobileControls = true;
            forceEnableMobileControlsButton.gameObject.SetActive(false);
            EnableMobileCanvas();
        }
#endif
    }
}
