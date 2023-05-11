using System;
using UnityEngine;

namespace Nightmares.Code.UI
{
    public class MobileInput : MonoBehaviour
    {
        [SerializeField] private PointerStatus left;
        [SerializeField] private PointerStatus right;
        [SerializeField] private PointerStatus up;

        public event Action onJump;
        
        public float HorizontalInput { get; private set; }
        public float VerticalInput { get; private set; }

        private void OnEnable() 
            => up.onPointerDown += OnJump;

        private void OnDisable() 
            => up.onPointerDown -= OnJump;

        private void OnJump() 
            => onJump?.Invoke();

        private void Update()
        {
            HorizontalInput = this switch
            {
                {left: var l} when l.Held => -1f,
                {right: var r} when r.Held => 1f,
                _ => 0f
            };

            VerticalInput = up.Held ? 1f : 0f;
        }
    }
}
