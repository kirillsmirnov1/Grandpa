using UnityEngine;

namespace Nightmares.Code.Model
{
    public class TouchControlsEnabler : MonoBehaviour
    {
        public static bool TouchControlsEnabled 
            => _instance.forceMobileControls || Application.platform == RuntimePlatform.Android;
        
        private static TouchControlsEnabler _instance;
        
        public bool forceMobileControls = false;

        [SerializeField] private GameObject mobileCanvas;
        
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
            mobileCanvas.SetActive(TouchControlsEnabled);
        }
    }
}
