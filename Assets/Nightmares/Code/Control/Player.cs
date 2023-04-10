using System;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class Player : MonoBehaviour
    {
        public static event Action OnPlayerDeath; 
        public static event Action<int> OnPlayerHealthChange; 

        [SerializeField] private int maxHealth = 3;
        
        public static Player Instance { get; private set; }

        public int Health
        {
            get => _health;
            private set
            {
                if(value == _health) return;
                _health = value;
                OnPlayerHealthChange?.Invoke(_health);
            }
        }

        private int _health;

        private void Awake()
        {
            Instance = this;
            Health = maxHealth;
        }

        public void DoDamage()
        {
            Health--;
            if (Health <= 0)
            {
                OnPlayerDeath?.Invoke();
                Debug.Log("Player is dead");
                gameObject.SetActive(false);
            }
        }
    }
}