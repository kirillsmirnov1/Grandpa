using System.Collections.Generic;
using Nightmares.Code.Model;
using UnityEngine;
using UnityEngine.Localization;
using UnityUtils.Variables;
using UnityUtils.View;

namespace Nightmares.Code.UI.Story
{
    public class StoryView : ListView<StoryEntryData>
    {
        [SerializeField] private List<StoryEntryData> dataEntries;
        [SerializeField] private CenterScroll centerScroll;
        [SerializeField] private StringArrayVariable completedQuests;
        [SerializeField] private LocalizedString lockedStoriesPrompts;
        
        private bool _resizedEntries;
        
        private void OnEnable()
        {
            var prompts = lockedStoriesPrompts.GetLocalizedString().Split("\n");
            
            var questsCompleted = completedQuests.Length;
            for (int i = 0; i < dataEntries.Count; i++)
            {
                var de = dataEntries[i];
                de.unlocked = i < questsCompleted;
                if (!de.unlocked)
                {
                    de.lockedPrompt = prompts[Random.Range(0, prompts.Length)];
                }
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

        protected override void SetEntries(List<StoryEntryData> data)
        {
            base.SetEntries(data);
            Resize();
        }

        private void Resize()
        {
            if(_resizedEntries) return;
            _resizedEntries = true;
            
            var parentsAspect = Screen.width * 1f / Screen.height;
            if (parentsAspect < .7f)
            {
                var firstEntry = entries[0];
                var newWidth = ((StoryViewEntry)firstEntry).rect.rect.height * parentsAspect;
                foreach (var entry in entries)
                {
                    ((StoryViewEntry)entry).rect
                        .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                }
            }
        }
    }
}