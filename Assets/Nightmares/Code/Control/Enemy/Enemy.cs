using System;
using UnityEngine;
using UnityUtils.Variables;

namespace Nightmares.Code.Control.Enemy
{
    public class Enemy : MonoBehaviour
    {
        public event Action<float> OnEnemyHealthChange;
        public static event Action OnEnemyDamaged;
        public static event Action<Enemy> OnEnemyDestroyed;
        
        [SerializeField] private int startHealth = 1;
        [SerializeField] private int points;
        [SerializeField] private bool throwbacksPlayerOnAttack = false;
        [SerializeField] private bool thrownBackByPlayerAttack = true;
        [SerializeField] private float attackFromTopAngle = 90f;
        
        [Header("Components")]
        [SerializeField] private Invincibility invincibility;
        public Rigidbody2D rb;
        
        [Header("Variables")]
        [SerializeField] private IntVariable[] incrementOnDeath;

        public int Points => points;
        public float AttackFromTopAngle => attackFromTopAngle / 2f; 
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

        public bool CanBeDamaged => invincibility != null && invincibility.CanBeDamaged;

        private int _health;

        protected virtual void OnEnable()
        {
            Health = startHealth;
        }

        public virtual void Damage()
        {
            Health--;
            OnEnemyDamaged?.Invoke();
            if (Health <= 0)
            {
                Kill();
            }
            else
            {
                StartInvincibleState();
            }
        }

        private void Kill()
        {
            foreach (var variable in incrementOnDeath)
            {
                variable.Value++;
            }
            
            OnEnemyDestroyed?.Invoke(this);
            Destroy(gameObject);
        }

        public void StartInvincibleState()
        {
            invincibility.StartInvincibleState();
        }
    }
}
