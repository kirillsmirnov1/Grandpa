using System.Linq;
using Nightmares.Code.Extensions;
using Nightmares.Code.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityUtils.Variables;
using UnityUtils.View;

namespace Nightmares.Code.UI.Story
{
    public class StoryViewEntry : ListViewEntry<StoryEntryData>
    {
        [SerializeField] public RectTransform rect;
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private Image blurObject;
        [SerializeField] private TextMeshProUGUI lockPrompt;
        [SerializeField] private LocalizedString mainTextSpacingStr;
        [SerializeField] private IntVariable maxSeenStory;
        
        private StoryEntryData data;
        private bool shouldPerformVisibilityCheck = true;

        private void Update()
        {
            if(shouldPerformVisibilityCheck) PerformVisibilityCheck();
        }

        private void PerformVisibilityCheck()
        {
            if (rect.IsVisibleFrom(Camera.main))
            {
                // Object became visible, time to check things
                shouldPerformVisibilityCheck = false;
                if (transform.GetSiblingIndex() > maxSeenStory.Value)
                {
                    maxSeenStory.Value = transform.GetSiblingIndex();
                }
            }
        }

        public override void Fill(StoryEntryData newData)
        {
            this.data = newData;
            header.text = newData.title.GetLocalizedString();
            mainText.text = newData.mainText.GetLocalizedString();
            lockPrompt.text = newData.lockedPrompt;

            SetSpacings();

            blurObject.gameObject.SetActive(!newData.unlocked);
            base.Fill(newData);

            shouldPerformVisibilityCheck = data.unlocked;

#if UNITY_WEBGL
            blurObject.material = null;
#endif
        }

        private void SetSpacings()
        {
            // TODO there must be a better way to do this 
            var spacings = mainTextSpacingStr
                .GetLocalizedString()
                .Split(",")
                .Select(float.Parse)
                .ToArray();

            mainText.characterSpacing = spacings[0];
            mainText.wordSpacing = spacings[1];
        }
    }
}