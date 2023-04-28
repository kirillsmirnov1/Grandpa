using System.Collections.Generic;
using DG.Tweening;
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

        [Header("Damage Handling")] 
        [SerializeField] private Vector2 playerThrowForce;
        [SerializeField] private int fliesToSpawnOnHit = 5;
        [SerializeField] private GameObject flyPrefab; // TODO use factory later
        [SerializeField] private Transform flySpawnAnchor;
        
        private State _state;
        private List<Enemy> _spawnedFlies;

        private void Start()
        {
            StartState(new IdleMovement(this));
            mainSprite.onBecameVisible += OnVisible;
            enemyRef.OnEnemyHealthChange += UpdateSlider;
        }

        private void OnEnable()
        {
            _spawnedFlies = new List<Enemy>();
            enemyRef.OnEnemyHealthChange += OnHealthChange;
        }

        private void OnDisable()
        {
            enemyRef.OnEnemyHealthChange -= OnHealthChange;
        }

        private void FixedUpdate()
        {
            _state?.FixedUpdate();
        }

        private void OnHealthChange(float newHealth)
        {
            if (newHealth == 0f) DestroySpawnedFlies();
            if(newHealth is 1f or 0f) return;
            SpawnFlies();
            ThrowPlayerOut();
        }

        private void DestroySpawnedFlies()
        {
            var seq = DOTween.Sequence();
            foreach (var spawnedFly in _spawnedFlies)
            {
                if(spawnedFly == null) continue;
                seq.AppendInterval(.05f);
                seq.AppendCallback(() => spawnedFly?.Damage());
            }
        }

        private void ThrowPlayerOut()
        {
            var throwForce = new Vector2(
                (transform.position.x > 0 ? -1 : 1) * playerThrowForce.x, 
                    playerThrowForce.y);
            Player.Instance.GetComponent<PlayerMovement>().Throw(throwForce);
        }

        private void SpawnFlies()
        {
            var seq = DOTween.Sequence();
            for (int i = 0; i < fliesToSpawnOnHit; i++)
            {
                seq.AppendInterval(.05f);
                seq.AppendCallback(() =>
                {
                    var fly = Instantiate(flyPrefab, flySpawnAnchor.position, Quaternion.identity)
                        .GetComponent<Enemy>();
                    fly.StartInvincibleState();
                    _spawnedFlies.Add(fly);
                });
            }
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
                var force = (playerPos - selfPos).normalized * Ctx.staffThrowForce;
                Ctx.staffRb.AddForce(force, ForceMode2D.Impulse);
            }

            public override void OnStaffReturned()
            {
                Ctx.StartState(new IdleMovement(Ctx, _direction));
            }
        }
    }
}