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
            
            public Webbing(EnemySpiderMovement context) : base(context) { }

            public override void Start()
            {
                var hit = Physics2D.Raycast(
                    Ctx.transform.position, 
                    Vector2.up, 
                    float.PositiveInfinity, 
                    Ctx.webConnectionTarget);
             
                if (hit.collider != null)
                {
                    _targetPos = hit.point;
                }
                else
                {
                    _targetPos = new Vector2(0f, float.PositiveInfinity);
                }
            }

            public override void FixedUpdate()
            {
                Ctx.lineRenderer.SetPositions(new []{Ctx.transform.position, _targetPos});
                var targetVelocity = (_targetPos - Ctx.transform.position).normalized * Ctx.webbingSpeed; 
                Ctx.rb.velocity = Vector2.Lerp(Ctx.rb.velocity, targetVelocity, Time.fixedDeltaTime * Ctx.webbingAcceleration);
                // TODO check on target distance
            }
        }
    }
}
