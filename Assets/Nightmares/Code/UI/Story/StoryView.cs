using System.Collections.Generic;
using Nightmares.Code.Model;
using UnityEngine;
using UnityUtils.Variables;
using UnityUtils.View;

namespace Nightmares.Code.UI.Story
{
    public class StoryView : ListView<StoryEntryData>
    {
        [SerializeField] private List<StoryEntryData> dataEntries;
        [SerializeField] private CenterScroll centerScroll;
        [SerializeField] private StringArrayVariable completedQuests;
        
        private void OnEnable()
        {
            var questsCompleted = completedQuests.Length;
            for (int i = 0; i < dataEntries.Count; i++)
            {
                var de = dataEntries[i];
                de.unlocked = i < questsCompleted;
                dataEntries[i] = de;
            }
            SetEntries(dataEntries);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                centerScroll.ScrollBy(-1);
            } 
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                centerScroll.ScrollBy(1);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                centerScroll.ScrollTo(0);
            }
        }
    }
}