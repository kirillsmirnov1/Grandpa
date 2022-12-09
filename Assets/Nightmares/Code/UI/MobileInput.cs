using UnityEngine;

namespace Nightmares.Code.UI
{
    public class MobileInput : MonoBehaviour
    {
        [SerializeField] private PointerStatus left;
        [SerializeField] private PointerStatus right;
        [SerializeField] private PointerStatus up;
        
        public float HorizontalInput { get; private set; }
        public bool JumpInput => up.Down;

        private void Update()
        {
            HorizontalInput = this switch
            {
                {left: var l} when l.Held => -1f,
                {right: var r} when r.Held => 1f,
                _ => 0f
            };
        }
    }
}
