using UnityEngine;

namespace Nightmares.Code.Control
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 1f;
        public float jump = 1f;

        public float HorizontalInput { get; set; }
        public bool JumpInput { get; set; }
        
        private bool _onGround;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            GetInput();
            Move();
        }

        private void GetInput()
        {
            HorizontalInput = Input.GetAxis("Horizontal");
            JumpInput = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        }

        private void Move()
        {
            _rb.velocity = new Vector2(speed * HorizontalInput, _rb.velocity.y);

            if (_onGround && JumpInput)
            {
                _onGround = false;
                _rb.AddForce(new Vector2(_rb.velocity.x, jump), ForceMode2D.Impulse);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.GetContact(0).point.y < transform.position.y)
            {
                _onGround = true;
            }
        }
    }
}
