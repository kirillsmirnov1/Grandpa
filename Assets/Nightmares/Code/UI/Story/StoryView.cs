using System;
using System.Collections.Generic;
using Nightmares.Code.Model;
using UnityEngine;
using UnityUtils.View;

namespace Nightmares.Code.UI.Story
{
    public class StoryView : ListView<StoryEntryData>
    {
        [SerializeField] private List<StoryEntryData> dataEntries;
        [SerializeField] private CenterScroll centerScroll;
        
        private void OnEnable()
        {
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
            {
                
            }
        }
    }
}