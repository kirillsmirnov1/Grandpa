using System;
using UnityEngine;

namespace Nightmares.Code.Model
{
    [Serializable]
    public struct StoryEntryData
    {
        public string title;
        [TextArea(5, 25)]
        public string mainText;
    }
}