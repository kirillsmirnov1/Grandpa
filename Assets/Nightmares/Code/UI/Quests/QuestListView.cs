using System;
using System.Linq;
using DG.Tweening;
using Nightmares.Code.Model.Quests;
using UnityEngine;
using UnityUtils.Variables;
using UnityUtils.View;

namespace Nightmares.Code.UI.Quests
{
    public class QuestListView : ListView<QuestDisplayData>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panel;
        
        [Header("Resources")]
        [SerializeField] private QuestManager quests;
        [SerializeField] private IntVariable maxUnlockedLevel;

        [Header("Animation")]
        [SerializeField] private float animDuration = .3f;
        [Range(0f, 1f)]
        [SerializeField] private float lowering = .3f;

        private Sequence _animSeq;
        
        public void Show()
        {
            gameObject.SetActive(true);
            var allQuests = quests.quests;
            SetEntries(allQuests.Select(ToDisplayData).ToList());

            Animate(true);
        }

        public void Hide()
        {
            Animate(false, () => gameObject.SetActive(false));
        }

        private void Animate(bool show, Action callback = null) // TODO extract as a component 
        {
            _animSeq?.Complete();
         
            canvasGroup.alpha = show ? 0f : 1f;
            var dY = panel.rect.height * lowering;
            panel.anchoredPosition = show ? panel.anchoredPosition - new Vector2(0, dY) : Vector2.zero;
            
            _animSeq = DOTween.Sequence()
                .Append(canvasGroup.DOFade(show ? 1f : 0f, animDuration))
                .Join(panel.DOAnchorPosY(show ? 0 : -dY, animDuration))
                .AppendCallback(() => callback?.Invoke());
        }

        private QuestDisplayData ToDisplayData(Quest quest) => new() {
            Description = quest.displayName,
            IsUnlocked = quest.minLevel <= maxUnlockedLevel,
            IsCompleted = quest.IsCompleted
        };
    }
}
