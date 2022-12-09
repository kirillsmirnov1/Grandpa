using UnityEngine;

namespace Nightmares.Code.Model
{
    public class GameMode : MonoBehaviour
    {
        public static bool TouchControlsEnabled 
            => _instance.forceMobileControls || Application.platform == RuntimePlatform.Android;
        
        private static GameMode _instance;
        
        public bool forceMobileControls = false;
        
        private void Awake()
        {
            _instance = this;
        }
    }
}
