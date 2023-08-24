using System;
using UnityEngine.Localization;

namespace Nightmares.Code.Model
{
    [Serializable]
    public struct StoryEntryData
    {
        public LocalizedString title; 
        public LocalizedString mainText;
        public bool unlocked;
        public string lockedPrompt;
    }
}