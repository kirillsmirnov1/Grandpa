using System;
using DG.Tweening;
using UnityEngine;

namespace Nightmares.Code.UI
{
    public class PanelVisibilityAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panel;

        [Header("Animation")]
        [SerializeField] private float animDuration = .3f;
        [Range(0f, 1f)]
        [SerializeField] private float lowering = .3f;

        private Sequence _animSeq;

        public void Show() => Show(null);
        public void Show(Action callback) => Animate(true, callback);

        public void Hide() => Hide(null);
        public void Hide(Action callback)
            => Animate(false, () =>
            {
                gameObject.SetActive(false);
                callback?.Invoke();
            });
        
        private void Animate(bool show, Action callback = null) // TODO extract as a component 
        {
            _animSeq?.Complete();
            gameObject.SetActive(true);
         
            canvasGroup.alpha = show ? 0f : 1f;
            var dY = panel.rect.height * lowering;
            panel.anchoredPosition = show ? panel.anchoredPosition - new Vector2(0, dY) : Vector2.zero;
            
            _animSeq = DOTween.Sequence()
                .Append(canvasGroup.DOFade(show ? 1f : 0f, animDuration))
                .Join(panel.DOAnchorPosY(show ? 0 : -dY, animDuration))
                .AppendCallback(() => callback?.Invoke());
        }
    }
}