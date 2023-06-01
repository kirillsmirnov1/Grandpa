using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nightmares.Code.UI
{
    [RequireComponent(typeof(AspectRatioFitter))]
    public class OptionalAspectRatioFitter : MonoBehaviour
    {
        [SerializeField] private AspectRatioFitter fitter;
        [SerializeField] private RectTransform rect;
        [SerializeField] private Mode mode;
        
        private void OnValidate()
        {
            fitter ??= GetComponent<AspectRatioFitter>();
            rect ??= GetComponent<RectTransform>();
            
            if (fitter.aspectMode != AspectRatioFitter.AspectMode.HeightControlsWidth
                && fitter.aspectMode != AspectRatioFitter.AspectMode.WidthControlsHeight)
            {
                throw new ArgumentOutOfRangeException("", "Wrong mode");
            }

            if (!Application.isPlaying)
            {
                fitter.enabled = false;
            }
        }

        private void OnEnable()
        {
            var targetRatio = fitter.aspectRatio;
            var actualRatio = rect.rect.width / rect.rect.height;

            switch (mode)
            {
                case Mode.KeepIn:
                    if (actualRatio > targetRatio)
                    {
                        fitter.enabled = true;
                    }
                    break;
                case Mode.KeepOut:
                    if (actualRatio < targetRatio)
                    {
                        fitter.enabled = true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum Mode
        {
            KeepIn,
            KeepOut,
        }
    }
}
