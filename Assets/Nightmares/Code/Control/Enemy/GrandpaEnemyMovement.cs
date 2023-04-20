using Nightmares.Code.Extensions;
using Nightmares.Code.UI;
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

        [SerializeField] private HealthSlider healthSlider;
        [SerializeField] private Enemy enemyRef;
        
        private State _state;

        private void Start()
        {
            StartState(new IdleMovement(this));
            mainSprite.onBecameVisible += OnVisible;
            enemyRef.OnEnemyHealthChange += UpdateSlider;
        }

        private void FixedUpdate()
        {
            _state?.FixedUpdate();
        }

        private void OnVisible()
        {
            healthSlider.gameObject.SetActive(true);
            healthSlider.Init("Grandpa", 1f);
        }

        private void UpdateSlider(float newHealthPercent)
        {
            healthSlider.UpdateSlider(newHealthPercent);
            if(newHealthPercent == 0f) healthSlider.gameObject.SetActive(false);
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
                CheckOnPlayer();
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

            private void CheckOnPlayer()
            {
                if (EnemyUtils.CheckEnemySeesPlayer(Ctx.transform.position))
                {
                    Ctx.StartState(new ThrowStaff(Ctx));
                }
            }
        }

        private class ThrowStaff : State
        {
            public ThrowStaff(GrandpaEnemyMovement ctx) : base(ctx) { }

            public override void Start()
            {
                var playerPos = Player.Instance.transform.position;

                var direction = Ctx.transform.position.x - playerPos.x;
                Ctx.transform.rotation = Quaternion.Euler(0, direction > 0 ? 180 : 0, 0);
            }

            public override void FixedUpdate()
            {
                Ctx.rb.velocity = Vector2.zero;
            }
        }
    }
}