using System;
using DG.Tweening;
using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class Enemy : MonoBehaviour
    {
        public event Action<float> OnEnemyHealthChange;
        public static event Action<Vector3> OnEnemyDestroyed;
        
        [SerializeField] private int startHealth = 1;
        [SerializeField] private bool throwbacksPlayerOnAttack = false;
        [SerializeField] private bool thrownBackByPlayerAttack = true;
        [SerializeField] private SpriteRenderer sprite;
        
        public bool ThrowbacksPlayerOnAttack => throwbacksPlayerOnAttack; 
        public bool ThrownBackByPlayerAttack => thrownBackByPlayerAttack;

        private int Health
        {
            get => _health;
            set
            {
                if(value == _health) return;
                _health = value;
                OnEnemyHealthChange?.Invoke(1f * _health / startHealth);
            }
        }
        private int _health;
        
        public bool CanBeDamaged { get; private set; }
        
        public Rigidbody2D rb;

        protected virtual void OnEnable()
        {
            Health = startHealth;
        }

        public virtual void Damage()
        {
            Health--;
            if (Health <= 0)
            {
                OnEnemyDestroyed?.Invoke(transform.position);
                Destroy(gameObject);
            }
            else
            {
                StartInvincibleState();
            }
        }

        public void StartInvincibleState()
        {
            var defaultColor = sprite.color;
            var disabledColor = defaultColor;
            disabledColor.a = .1f;

            CanBeDamaged = false;
            DOTween.Sequence()
                .Append(sprite.DOColor(disabledColor, .1f))
                .Append(sprite.DOColor(defaultColor, .1f))
                .SetLoops(10)
                .AppendCallback(() => CanBeDamaged = true)
                ;
        }
    }
}
