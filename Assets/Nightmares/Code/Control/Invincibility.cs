using System;
using DG.Tweening;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class Invincibility : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite;

        public bool CanBeDamaged { get; private set; }

        private void OnEnable()
        {
            CanBeDamaged = true;
        }

        public void StartInvincibleState()
        {
            var defaultColor = sprite.color;
            var disabledColor = defaultColor;
            disabledColor.a = .1f;

            CanBeDamaged = false;
            DOTween.Sequence()
                .AppendCallback(() => CanBeDamaged = false)
                .Append(sprite.DOColor(disabledColor, .1f))
                .Append(sprite.DOColor(defaultColor, .1f))
                .SetLoops(10)
                .AppendCallback(() => CanBeDamaged = true)
                ;
        }
    }
}