using System;
using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class Enemy : MonoBehaviour
    {
        public static event Action<Vector3> OnEnemyDestroyed;
        
        [SerializeField] private int startHealth = 1;
        [SerializeField] private bool throwbacksPlayerOnAttack = false;
        [SerializeField] private bool thrownBackByPlayerAttack = true;

        public bool ThrowbacksPlayerOnAttack => throwbacksPlayerOnAttack; 
        public bool ThrownBackByPlayerAttack => thrownBackByPlayerAttack; 
        
        private int _health;
        
        public Rigidbody2D rb;

        private void OnEnable()
        {
            _health = startHealth;
        }

        public void Damage()
        {
            _health--;
            if (_health <= 0)
            {
                OnEnemyDestroyed?.Invoke(transform.position);
                Destroy(gameObject);
            }
        }
    }
}
