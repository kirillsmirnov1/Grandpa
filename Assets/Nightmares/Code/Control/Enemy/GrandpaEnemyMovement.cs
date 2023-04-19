using System;
using Nightmares.Code.Extensions;
using Unity.VisualScripting;
using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class GrandpaEnemyMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 1f;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Rigidbody2D staffRb;

        [SerializeField] private float idleWallCheckDistance = 1f;
        [SerializeField] private LayerMask idleLayersWallCheck;
        [SerializeField] private RenderExtensions mainSprite;
        
        private State _state;

        private void Start()
        {
            StartState(new IdleMovement(this));
            mainSprite.onBecameVisible += OnVisible;
        }

        private void FixedUpdate()
        {
            _state?.FixedUpdate();
        }

        private void OnVisible()
        {
            // TODO show ui
        }

        private void StartState(State newState)
        {
            _state = newState;
            _state.Start();
        }
        
        private abstract class State
        {
            protected GrandpaEnemyMovement Ctx;
            public State(GrandpaEnemyMovement ctx) => Ctx = ctx;
            public abstract void Start();
            public abstract void FixedUpdate();
        }
        
        private class IdleMovement : State
        {
            private float _direction = 1f;
            
            public IdleMovement(GrandpaEnemyMovement ctx) : base(ctx) { }

            public override void Start()
            {
                CheckDirection();
            }

            public override void FixedUpdate()
            {
                CheckDirection();
                Move();
            }

            private void CheckDirection()
            {
                var hit = Physics2D.Raycast(Ctx.transform.position, new Vector2(_direction, 0f),
                    Ctx.idleWallCheckDistance, Ctx.idleLayersWallCheck);

                if (hit.collider != null)
                {
                    _direction *= -1f;
                    Ctx.transform.Rotate(0, 180, 0);
                }
            }

            private void Move()
            {
                Ctx.rb.velocity = new Vector2(_direction * Time.fixedDeltaTime * Ctx.movementSpeed, 0);
            }
        }
    }
}