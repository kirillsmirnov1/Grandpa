﻿using System;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class Player : MonoBehaviour
    {
        public static event Action OnPlayerDeath;
        public static event Action OnPlayerDamage;
        public static event Action<int> OnPlayerHealthChange; 

        [SerializeField] private int maxHealth = 3;
        public Rigidbody2D rb;
        [SerializeField] private Invincibility invincibility;
        
        public static Player Instance { get; private set; }
        public bool CanBeDamaged => invincibility.CanBeDamaged;
        
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
        }

        private void Start()
        {
            Health = maxHealth;
        }

        public void DoDamage()
        {
            Health--;
            OnPlayerDamage?.Invoke();
            if (Health <= 0)
            {
                OnPlayerDeath?.Invoke();
                Debug.Log("Player is dead");
                gameObject.SetActive(false);
            }
            else
            {
                invincibility.StartInvincibleState();
            }
        }
    }
}