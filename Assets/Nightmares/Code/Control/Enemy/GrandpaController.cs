using System;
using System.Collections.Generic;
using DG.Tweening;
using Nightmares.Code.Extensions;
using Nightmares.Code.Model;
using Nightmares.Code.UI;
using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class GrandpaController : MonoBehaviour
    {
        public static event Action OnGrandpaDefeated;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D grandpaCollider;
        [SerializeField] private Enemy enemyRef;
        [SerializeField] private RenderExtensions mainSprite;

        [Header("Health Slider")]
        [SerializeField] private CanvasGroup healthSliderCanvasGroup;
        [SerializeField] private RectTransform healthSliderRect;
        [SerializeField] private HealthSlider healthSlider;
        
        [Header("Idle movement")]
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float minMovementDuration = 1f;
        [SerializeField] private float idleWallCheckDistance = 1f;
        [SerializeField] private LayerMask idleLayersWallCheck;
        [SerializeField] private float minVerticalDistanceToPlayer = 7;

        [Header("Staff throwing")]
        [SerializeField] private float staffThrowForce = 10f;
        [SerializeField] private Rigidbody2D staffRb;
        [SerializeField] private Collider2D staffCollider;
        [SerializeField] private EnemyStaff enemyStaff;
        [SerializeField] private float shakeDistance = .3f;
        [SerializeField] private float shakeDuration = 1f;
        [SerializeField] private int shakeVibrato = 30;
        
        [Header("Damage Handling")] 
        [SerializeField] private Vector2 playerThrowForce;
        [SerializeField] private GameObject flyPrefab; // TODO use factory later
        [SerializeField] private Transform flySpawnAnchor;

        private int _fliesToSpawnOnHit;
        private State _state;
        private List<Enemy> _spawnedFlies;

        public bool HealthBarShown { get; set; }

        private void Start()
        {
            _fliesToSpawnOnHit = Mathf.FloorToInt(Prefs.GrandpaDifficulty * 0.5f + 1.5f);
            mainSprite.onBecameVisible += OnVisible;
            enemyRef.OnEnemyHealthChange += UpdateSlider;
            Physics2D.IgnoreCollision(grandpaCollider, staffCollider);
        }

        private void OnEnable()
        {
            _spawnedFlies = new List<Enemy>();
            healthSlider.gameObject.SetActive(false);
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
            switch (newHealth)
            {
                case 0f:
                    DestroySpawnedFlies();
                    OnGrandpaDefeated?.Invoke();
                    break;
                case 1f:
                    break;
                default:
                    SpawnFlies();
                    ThrowPlayerOut();
                    break;
            }
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
            for (int i = 0; i < _fliesToSpawnOnHit; i++)
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
            StartState(new IdleMovement(this));
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

        private void ShowHealthBar()
        {
            if(HealthBarShown) return;
            HealthBarShown = true;
            
            healthSlider.gameObject.SetActive(true);
            healthSlider.Init("Grandpa", 1f);

            var anchorPosY = healthSliderRect.anchoredPosition.y;
            healthSliderCanvasGroup.alpha = 0;
            healthSliderRect.anchoredPosition -= new Vector2(0, healthSliderRect.rect.height);
            
            DOTween.Sequence()
                .Join(healthSliderCanvasGroup.DOFade(1f, 1f))
                .Join(healthSliderRect.DOAnchorPosY(anchorPosY, 1f));
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

                var pos = Ctx.transform.position;
                var playerPos = Player.Instance.transform.position;
                
                if (Mathf.Abs(pos.y - playerPos.y) < Ctx.minVerticalDistanceToPlayer 
                    && EnemyUtils.CheckEnemySeesPlayer(Ctx.transform.position))
                {
                    Ctx.ShowHealthBar();
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

                DOTween.Sequence()
                    .Append(Ctx.enemyStaff.transform.DOShakePosition(
                        Ctx.shakeDuration, 
                        Ctx.shakeDistance, 
                        Ctx.shakeVibrato))
                    .AppendCallback(() => ActuallyThrowStaff(playerPos, Ctx.transform.position));
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