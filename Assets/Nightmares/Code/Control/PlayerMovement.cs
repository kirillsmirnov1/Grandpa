using Nightmares.Code.Model;
using Nightmares.Code.UI;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 1f;
        public float jump = 1f;

        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private MobileInput mobileInput;
        [SerializeField] private float throwForceDecreaseSpeed = 100f;
        
        public float HorizontalInput { get; set; }
        public bool JumpInput { get; set; }
        
        private bool _onGround;
        private Rigidbody2D _rb;
        private Collider2D _collider;
        private Vector2 _throwForce;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            mobileInput.onJump += TryJump;
        }

        private void OnDisable()
        {
            mobileInput.onJump -= TryJump;
        }
        
        private void Update()
        {
            GetInput();
            Move();
        }

        private void FixedUpdate()
        {
            GroundCheck();
            _throwForce = Vector2.Lerp(_throwForce, Vector2.zero, Time.fixedDeltaTime * throwForceDecreaseSpeed);
        }

        private void GroundCheck()
        {
            var bounds = _collider.bounds;
            var hit = Physics2D.CircleCast(bounds.center,bounds.extents.y, Vector2.down, bounds.extents.y + .01f, playerLayer);
            _onGround = hit.collider != null;
        }

        private void GetInput()
        {
            HorizontalInput = Input.GetAxis("Horizontal");
            JumpInput = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);

            if (TouchControlsEnabler.TouchControlsEnabled)
            {
                HorizontalInput += mobileInput.HorizontalInput;
            }
        }

        private void Move()
        {
            _rb.velocity = new Vector2(speed * HorizontalInput, _rb.velocity.y) + _throwForce;

            if (JumpInput)
            {
                TryJump();
            }
        }

        private void TryJump()
        {
            if (_onGround)
            {
                _rb.AddForce(new Vector2(_rb.velocity.x, jump), ForceMode2D.Impulse);
            }
        }

        public void Throw(Vector2 throwForce)
        {
            _throwForce = throwForce;
        }
    }
}
