using System.Collections.Generic;
using Nightmares.Code.Model;
using UnityEngine;
using UnityUtils.View;

namespace Nightmares.Code.UI.Story
{
    public class StoryView : ListView<StoryEntryData>
    {
        [SerializeField] private List<StoryEntryData> dataEntries;
        
        private void OnEnable()
        {
            SetEntries(dataEntries);
        }
    }
}