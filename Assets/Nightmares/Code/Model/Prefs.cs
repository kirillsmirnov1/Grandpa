using UnityEngine;

namespace Nightmares.Code.Model
{
    public static class Prefs
    {
        private const string KeyGrandpaDifficulty = "KeyGrandpaDifficulty";

        public static int GrandpaDifficulty
        {
            get => PlayerPrefs.GetInt(KeyGrandpaDifficulty, 1);
            set => PlayerPrefs.SetInt(KeyGrandpaDifficulty, Mathf.Max(1, value));
        }
    }
}