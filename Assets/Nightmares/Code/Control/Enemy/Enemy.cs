using System;
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
        }
    }
}
