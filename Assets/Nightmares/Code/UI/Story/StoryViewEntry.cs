using System.Linq;
using Nightmares.Code.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityUtils.View;

namespace Nightmares.Code.UI.Story
{
    public class StoryViewEntry : ListViewEntry<StoryEntryData>
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private GameObject blurObject;
        [SerializeField] private LocalizedString mainTextSpacingStr;

        private void OnEnable()
        {
            var viewRect = transform.parent.parent.GetComponent<RectTransform>();
            var targetSize = viewRect.rect.size;
            var parentsAspect = targetSize.x / targetSize.y;
            if (parentsAspect < .7f)
            {
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.height * parentsAspect);
            }
        }

        public override void Fill(StoryEntryData data)
        {
            header.text = data.title.GetLocalizedString();
            mainText.text = data.mainText.GetLocalizedString();

            SetSpacings();

            blurObject.SetActive(!data.unlocked);
            base.Fill(data);
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