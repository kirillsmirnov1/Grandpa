using Unity.Mathematics;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class CharControlPlatformer : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 1;
        [SerializeField] private float moveForce = 1;
        [SerializeField] private float maxMoveSpeed = 10;
        
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                _rb.AddForce(jumpForce * Input.GetAxis("Vertical") * Vector2.up, ForceMode2D.Impulse);
            }

            var horInput = Input.GetAxis("Horizontal");
            var horizontalInputUnsigned = math.abs(horInput) * moveForce;
            var horizontalSpeed = Mathf.Abs(_rb.velocity.x);
            var desiredSpeed = Mathf.Clamp(horizontalInputUnsigned, 0, maxMoveSpeed - horizontalSpeed);
            _rb.AddForce(new Vector2(desiredSpeed * Mathf.Sign(horInput), 0), ForceMode2D.Force);
        }
    }
}
