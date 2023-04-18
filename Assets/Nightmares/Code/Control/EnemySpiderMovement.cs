using Nightmares.Code.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nightmares.Code.Control
{
    public class EnemySpiderMovement : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Rigidbody2D rb;
        
        [Header("Webbing")]
        [SerializeField] private LayerMask webConnectionTarget;
        [SerializeField] private float webbingSpeed = 1f;
        [SerializeField] private float webbingAcceleration = 1f;
        [SerializeField] private float targetDistance = 3f;
        [SerializeField] private float minWebbingHeight = 1;

        [Header("Swinging")]
        [SerializeField] private float swingLimitX = 1.5f;
        [SerializeField] private float swingSpeed = 30f;
        [SerializeField] private Vector2 swingAcceleration = new Vector2(.5f, .9f);
        [SerializeField] private float wallCheckDist = .6f;
        [SerializeField] private float maxJumpDistance = 5f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float groundCheckHeight = .5f;
        
        private SpiderMovementState _state;

        private void Start()
        {
            lineRenderer.positionCount = 2;
            StartGoingUpOnWebState();
        }

        private void FixedUpdate()
        {
            _state?.FixedUpdate();
        }

        private void OnDrawGizmos()
        {
            _state?.OnDrawGizmos();
        }

        private void StartGoingUpOnWebState()
        {
            StartState(new GoingUpOnWeb(this));
        }

        private void StartSwingingState(Vector3 targetPos, float targetHeight)
        {
            StartState(new Swinging(this, targetPos, targetHeight));
        }

        private void StartJumpState()
        {
            StartState(new Jump(this));
        }

        private void StartState(SpiderMovementState newState)
        {
            _state = newState;
            _state.Start();
        }

        private abstract class SpiderMovementState
        {
            protected EnemySpiderMovement Ctx { get; }

            protected SpiderMovementState(EnemySpiderMovement context)
            {
                Ctx = context;
            }

            public abstract void Start();
            public abstract void FixedUpdate();
            public virtual void OnDrawGizmos(){}
        }

        private class GoingUpOnWeb : SpiderMovementState
        {
            private Vector3 _targetPos;
            private bool _hasTarget;
            
            public GoingUpOnWeb(EnemySpiderMovement context) : base(context) { }

            public override void Start()
            {
                Ctx.lineRenderer.enabled = false;
                FindTargetPos();
            }

            public override void FixedUpdate()
            {
                if (!_hasTarget)
                {
                    FindTargetPos();
                    return;
                }
                
                Ctx.lineRenderer.SetPositions(new[] { Ctx.transform.position, _targetPos });

                ApproachTarget();
            }

            private void FindTargetPos()
            {
                var hit = Physics2D.Raycast(
                    Ctx.transform.position,
                    Vector2.up,
                    float.PositiveInfinity,
                    Ctx.webConnectionTarget);

                if (hit.collider != null)
                {
                    _targetPos = hit.point;
                    _hasTarget = true;
                    Ctx.lineRenderer.enabled = true;
                }
            }

            private void ApproachTarget()
            {
                var toTarget = _targetPos - Ctx.transform.position;
                var webIsLong = toTarget.magnitude > Ctx.targetDistance;
                var groundHit = Physics2D.Raycast(Ctx.transform.position, Vector2.down, Ctx.minWebbingHeight,
                    Ctx.webConnectionTarget);
                var closeToGround = groundHit.collider != null;
                
                if (webIsLong || closeToGround)
                {
                    MoveToTarget(toTarget);
                }
                else
                {
                    Ctx.StartSwingingState(_targetPos, _targetPos.y - Ctx.transform.position.y);
                }
            }

            private void MoveToTarget(Vector3 toTarget)
            {
                var targetVelocity = toTarget.normalized * Ctx.webbingSpeed;
                Ctx.rb.velocity = Vector2.Lerp(Ctx.rb.velocity, targetVelocity,
                    Time.fixedDeltaTime * Ctx.webbingAcceleration);
            }
        }

        private class Swinging : SpiderMovementState
        {
            private readonly Vector3 _topTargetPos;
            private readonly float _targetHeight;
            
            private Vector3 _nextTargetPos;
            private bool _goingRight;

            private readonly Transform _player;
            
            public Swinging(EnemySpiderMovement context, Vector3 targetPos, float targetHeight) : base(context)
            {
                _topTargetPos = targetPos;
                _targetHeight = targetHeight;
                _player = Player.Instance.transform;
            }

            public override void Start()
            {
                ChangeNextTargetPos();
            }

            public override void FixedUpdate()
            {
                var toTarget = _nextTargetPos - Ctx.transform.position;
                var targetVelocity = toTarget.normalized * Ctx.swingSpeed;
                Ctx.rb.velocity = new Vector2(
                    Mathf.Lerp(Ctx.rb.velocity.x, targetVelocity.x, Time.fixedDeltaTime * Ctx.swingAcceleration.x),
                    Mathf.Lerp(Ctx.rb.velocity.y, targetVelocity.y, Time.fixedDeltaTime * Ctx.swingAcceleration.y));;

                if (ShouldChangeDirections())
                {
                    ChangeNextTargetPos();
                }
                
                Ctx.lineRenderer.SetPositions(new[] { Ctx.transform.position, _topTargetPos });

                if (ShouldJump)
                {
                    Ctx.StartJumpState();
                }
            }

            private bool ShouldJump
            {
                get
                {
                    var pos = Ctx.transform.position;
                    var playerPos = _player.position;
                    
                    return playerPos.y < pos.y
                           && Vector2.Distance(playerPos, pos) < Ctx.maxJumpDistance
                           && EnemyUtils.CheckEnemySeesPlayer(pos);
                }
            }

            public override void OnDrawGizmos()
            {
                base.OnDrawGizmos();
                Gizmos.color = Color.white;
                Gizmos.DrawLine(Ctx.transform.position, _nextTargetPos);

                if (ShouldJump)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(Ctx.transform.position, _player.position);
                }
            }

            private bool ShouldChangeDirections()
            {
                var limitReached = Mathf.Abs(Ctx.transform.position.x - _nextTargetPos.x) <= .1f;
                var wallReached = Physics2D.Raycast(
                        Ctx.transform.position,
                        _goingRight ? Vector2.right : Vector2.left,
                        Ctx.wallCheckDist,
                        Ctx.webConnectionTarget)
                    .collider != null;
                
                return limitReached || wallReached;
            }

            private void ChangeNextTargetPos()
            {
                if (_goingRight)
                {
                    _nextTargetPos = _topTargetPos + new Vector3(-Ctx.swingLimitX, -_targetHeight/2f);
                    _goingRight = false;
                }
                else
                {
                    _nextTargetPos = _topTargetPos + new Vector3(Ctx.swingLimitX, -_targetHeight/2f);
                    _goingRight = true;
                }
            }
        }

        private class Jump : SpiderMovementState
        {
            public Jump(EnemySpiderMovement context) : base(context) { }

            public override void Start()
            {
                Ctx.lineRenderer.enabled = false;
                var toPlayer = Player.Instance.transform.position - Ctx.transform.position;
                Ctx.rb.velocity = Vector2.zero;
                Ctx.rb.AddForce(toPlayer * Ctx.jumpForce, ForceMode2D.Impulse);
            }

            public override void FixedUpdate()
            {
                var groundHit = Physics2D.Raycast(Ctx.transform.position, Vector2.down, Ctx.groundCheckHeight,
                    Ctx.webConnectionTarget);

                if (groundHit)
                {
                    Ctx.StartGoingUpOnWebState();
                }
            }
        }
    }
}
