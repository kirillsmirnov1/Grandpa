using Nightmares.Code.Extensions;
using Nightmares.Code.UI;
using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class GrandpaController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private HealthSlider healthSlider;
        [SerializeField] private Enemy enemyRef;
        [SerializeField] private RenderExtensions mainSprite;
        
        [Header("Idle movement")]
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float minMovementDuration = 1f;
        [SerializeField] private float idleWallCheckDistance = 1f;
        [SerializeField] private LayerMask idleLayersWallCheck;
        
        [Header("Staff throwing")]
        [SerializeField] private float staffThrowForce = 10f;
        [SerializeField] private Rigidbody2D staffRb;
        [SerializeField] private EnemyStaff enemyStaff;
        
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

        public void OnStaffReturned()
        {
            _state?.OnStaffReturned();
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
            protected GrandpaController Ctx;
            public State(GrandpaController ctx) => Ctx = ctx;
            public abstract void Start();
            public abstract void FixedUpdate();

            public virtual void OnStaffReturned() { }
        }

        private class IdleMovement : State
        {
            private float _direction = 1f;
            private float _movementStartTime;

            public IdleMovement(GrandpaController ctx, float direction = 1f) : base(ctx)
            {
                _direction = direction;
            }

            public override void Start()
            {
                _movementStartTime = Time.time;
                Ctx.enemyStaff.enabled = false;
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
                if(Time.time - _movementStartTime < Ctx.minMovementDuration) return;
                
                if (EnemyUtils.CheckEnemySeesPlayer(Ctx.transform.position))
                {
                    Ctx.StartState(new ThrowStaff(Ctx));
                }
            }
        }

        private class ThrowStaff : State
        {
            private float _direction;
            
            public ThrowStaff(GrandpaController ctx) : base(ctx) { }

            public override void Start()
            {
                var playerPos = Player.Instance.transform.position;
                var selfPos = Ctx.transform.position;

                _direction = Mathf.Sign(playerPos.x - selfPos.x);
                Ctx.transform.rotation = Quaternion.Euler(0, _direction > 0 ? 0 : 180, 0);

                ActuallyThrowStaff(playerPos, selfPos);
            }

            public override void FixedUpdate()
            {
                Ctx.rb.velocity = Vector2.zero;
            }

            private void ActuallyThrowStaff(Vector3 playerPos, Vector3 selfPos)
            {
                Ctx.staffRb.simulated = true;
                Ctx.enemyStaff.enabled = true;
                var force = (playerPos - selfPos) * Ctx.staffThrowForce;
                Ctx.staffRb.AddForce(force, ForceMode2D.Impulse);
            }

            public override void OnStaffReturned()
            {
                Ctx.StartState(new IdleMovement(Ctx, _direction));
            }
        }
    }
}