using UnityEngine;

namespace Nightmares.Code.Control
{
    public class EnemySpiderMovement : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        private SpiderMovementState _state;

        private void Start()
        {
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
            protected EnemySpiderMovement Ctx { get; private set; }

            protected SpiderMovementState(EnemySpiderMovement context)
            {
                Ctx = context;
            }

            public abstract void Start();
            public abstract void FixedUpdate();
        }

        private class Webbing : SpiderMovementState
        {
            public Webbing(EnemySpiderMovement context) : base(context) { }

            public override void Start()
            {
                Debug.Log($"{Ctx.gameObject.name} starts webbing");
                // TODO find point to connect to 
            }

            public override void FixedUpdate()
            {
                // TODO move up
                // TODO update line renderer 
            }
        }
    }
}
