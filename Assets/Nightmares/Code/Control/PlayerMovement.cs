using UnityEngine;

namespace Nightmares.Code.Control
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 1f;
        public float jump = 1f;

        private float _move;
        private bool _onGround;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _move = Input.GetAxis("Horizontal");
            _rb.velocity = new Vector2(speed * _move, _rb.velocity.y);

            if (_onGround && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
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
