using UnityEngine;

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
        [SerializeField] private float minHeight = .3f;
        
        private SpiderMovementState _state;

        private void Start()
        {
            lineRenderer.positionCount = 2;
            StartWebbingState();
        }

        private void FixedUpdate()
        {
            _state?.FixedUpdate();
        }

        private void StartWebbingState()
        {
            StartState(new Webbing(this));
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
        }

        private class Webbing : SpiderMovementState
        {
            private Vector3 _targetPos;
            private bool _hasTarget;
            
            public Webbing(EnemySpiderMovement context) : base(context) { }

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
                var groundHit = Physics2D.Raycast(Ctx.transform.position, Vector2.down, Ctx.minHeight,
                    Ctx.webConnectionTarget);
                var closeToGround = groundHit.collider != null;
                
                if (webIsLong || closeToGround)
                {
                    MoveToTarget(toTarget);
                }
                else
                {
                    // TODO go to next state
                }
            }

            private void MoveToTarget(Vector3 toTarget)
            {
                var targetVelocity = toTarget.normalized * Ctx.webbingSpeed;
                Ctx.rb.velocity = Vector2.Lerp(Ctx.rb.velocity, targetVelocity,
                    Time.fixedDeltaTime * Ctx.webbingAcceleration);
            }
        }
    }
}
