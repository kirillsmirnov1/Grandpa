using UnityEngine;

namespace Nightmares.Code.Control
{
    public class FlyEnemyMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 1;
        [SerializeField] private float dirResetTime = 2f;

        private Rigidbody2D _rb;
        private float _nextDirectionChange;
        private Vector2 _direction;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            InitialDirectionChange();
        }

        private void Update()
        {
            CheckDirectionUpdate();
            Move();
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
    }
}