using UnityEngine;

namespace Nightmares.Code.Model
{
    public static class Prefs
    {
        private const string KeyGrandpaDifficulty = "KeyGrandpaDifficulty";
        private const string KeyForceEnableMobileControls = "KeyForceEnableMobileControls";
        
        public static int GrandpaDifficulty
        {
            get => PlayerPrefs.GetInt(KeyGrandpaDifficulty, 1);
            set => PlayerPrefs.SetInt(KeyGrandpaDifficulty, Mathf.Max(1, value));
        }

        public static bool ForcedMobileControls
        {
            get => PlayerPrefs.GetInt(KeyForceEnableMobileControls, 0) == 1;
            set => PlayerPrefs.SetInt(KeyForceEnableMobileControls, value ? 1 : 0);
        }
    }
}