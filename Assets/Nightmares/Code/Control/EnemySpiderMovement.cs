using UnityEngine;

namespace Nightmares.Code.Control
{
    public class EnemySpiderMovement : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        
        [Header("Webbing")]
        [SerializeField] private LayerMask webConnectionTarget;
        [SerializeField] private float webbingSpeed = 1f;

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
                // TODO move up
                // TODO check on target distance
            }
        }
    }
}
