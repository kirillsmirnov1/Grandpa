using System;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class Enemy : MonoBehaviour
    {
        public static event Action<Vector3> OnEnemyDestroyed;
        
        [SerializeField] private int startHealth = 1;

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
