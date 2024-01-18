using DG.Tweening;
using Nightmares.Code.Control;
using UnityEngine;
using UnityUtils.Variables;

namespace Nightmares.Code.UI
{
    public class PulsateWhenHasUnreadStories : MonoBehaviour
    {
        [SerializeField] private IntVariable maxSeenStory;
        [SerializeField] private Vector2 sizeFromTo = new Vector2(1, 1.5f);
        [SerializeField] private float tweenDuration = .5f;
        [SerializeField] private Ease ease = Ease.InCubic;
        
        private bool _runningAnimation;
        private Sequence _seq;

        private bool ShouldPulsate 
            => maxSeenStory.Value < PlatformerGameManager.Instance.StoriesUnlocked - 1;
        
        private void OnEnable()
        {
            OnMaxSeenStoryChange();
            maxSeenStory.OnChangeBase += OnMaxSeenStoryChange;
        }

        private void OnDisable()
        {
            maxSeenStory.OnChangeBase -= OnMaxSeenStoryChange;
        }

        private void OnMaxSeenStoryChange()
        {
            if (ShouldPulsate)
            {
                if (!_runningAnimation)
                {
                    _seq = DOTween.Sequence()
                        .Append(transform.DOScale(sizeFromTo.y, tweenDuration).SetEase(ease))
                        .Append(transform.DOScale(sizeFromTo.x, tweenDuration).SetEase(ease))
                        .SetLoops(-1);
                    _seq.onKill = () => transform.localScale = Vector3.one * sizeFromTo.x;
                }
            }
            else
            {
                if (_runningAnimation)
                {
                    _seq.Kill();
                }
            }
        }
    }
}