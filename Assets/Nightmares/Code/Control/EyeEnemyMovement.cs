using Nightmares.Code.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nightmares.Code.Control
{
    public class EyeEnemyMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 1;
        [SerializeField] private float dirResetTime = 2f;
        [SerializeField] private float eyeRotationSpeed = 3f;
        
        private Rigidbody2D _rb;
        private float _nextDirectionChange;
        private Vector2 _direction;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            InitialDirectionChange();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, _direction);
        }

        private void Update()
        {
            CheckDirectionUpdate();
            RotateEye();
            Move();
        }

        private void RotateEye()
        {
            var desiredRotation = Quaternion.Euler(0, 0, _direction.ToAngleInDegrees()); 
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, eyeRotationSpeed * Time.deltaTime);
        }

        private void CheckDirectionUpdate()
        {
            if (Time.time > _nextDirectionChange)
            {
                ChangeDirection();
            }
        }

        private void Move()
        {
            var velocity = _rb.velocity;
            var velocityDiff = _direction - velocity;
            var velocityChange = velocityDiff * speed * Time.deltaTime;
            _rb.velocity += velocityChange;
        }

        private void InitialDirectionChange()
        {
            ChangeDirection();
            _nextDirectionChange = Time.time + Random.Range(dirResetTime / 3f, dirResetTime);
        }

        private void ChangeDirection()
        {
            _direction = Random.insideUnitCircle.normalized;
            _nextDirectionChange = Time.time + dirResetTime;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            ChangeDirection();
        }
    }
}