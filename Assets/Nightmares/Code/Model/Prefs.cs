using Nightmares.Code.Control;
using UnityEngine;

namespace Nightmares.Code.Model
{
    public static class Prefs
    {
        private const string KeyGrandpaDifficulty = "KeyGrandpaDifficulty";
        private const string KeyGrandpaDifficultyMax = "KeyGrandpaDifficultyMax";

        public static int GrandpaDifficulty
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt(KeyGrandpaDifficulty, 1), 1, Constants.GrandpaMaxDifficulty);
            set => PlayerPrefs.SetInt(KeyGrandpaDifficulty, Mathf.Clamp(value, 1, Constants.GrandpaMaxDifficulty));
        }
        
        public static int GrandpaDifficultyMaxUnlocked
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt(KeyGrandpaDifficultyMax, 1), 1, Constants.GrandpaMaxDifficulty);
            set => PlayerPrefs.SetInt(KeyGrandpaDifficultyMax, Mathf.Clamp(value, 1, Constants.GrandpaMaxDifficulty));
        }
    }
}