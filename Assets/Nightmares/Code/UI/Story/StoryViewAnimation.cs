using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nightmares.Code.UI.Story
{
    public class StoryViewAnimation : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private Ease viewportEase;
        [SerializeField] private float duration;

        [Header("Components")]
        [SerializeField] private RectTransform viewport;
        [SerializeField] private CanvasGroup buttonFade;
        [SerializeField] private Image background;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private CenterScroll centerScroll;
        
        [ContextMenu("Show")]
        public void Show()
        {
            var anchorPos = viewport.anchoredPosition;
            viewport.anchoredPosition = anchorPos - Vector2.up * viewport.rect.height;
            eventSystem.gameObject.SetActive(false);
            
            var c = background.color;
            c.a = 0f;
            background.color = c;
            buttonFade.alpha = 0;

            gameObject.SetActive(true);

            var seq = DOTween.Sequence();
            seq.Join(buttonFade.DOFade(1f, duration));
            seq.Join(background.DOFade(1f, duration));
            seq.Join(viewport.DOAnchorPosY(anchorPos.y, duration).SetEase(viewportEase));
            seq.AppendCallback(() =>
            {
                eventSystem.gameObject.SetActive(true);
                centerScroll.Init();
            });
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            var anchorPos = viewport.anchoredPosition;
            eventSystem.gameObject.SetActive(false);

            DOTween.Sequence()
                .Join(buttonFade.DOFade(0f, duration))
                .Join(background.DOFade(0f, duration))
                .Join(viewport.DOAnchorPosY(-viewport.rect.height, duration).SetEase(viewportEase))
                .AppendCallback(() =>
                {
                    viewport.anchoredPosition = anchorPos;
                    gameObject.SetActive(false);
                    eventSystem.gameObject.SetActive(true);
                });
        }
    }
}
