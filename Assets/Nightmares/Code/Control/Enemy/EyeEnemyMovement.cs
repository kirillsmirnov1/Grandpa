using Nightmares.Code.Extensions;
using Nightmares.Code.Model;
using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class EyeEnemyMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 1;
        [SerializeField] private float dirResetTime = 2f;
        [SerializeField] private float maxPlayerDistance = 5f;
        
        [Header("Iris")]
        [SerializeField] private float eyeRotationSpeed = 3f;
        [SerializeField] private float magnitude = .1f;
        [SerializeField] private Transform iris;
        
        private Rigidbody2D _rb;
        private float _nextDirectionChange;
        private Vector2 _direction;
        private Transform _player;
        private bool _playerVisible;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _player = Player.Instance.transform;
            InitialDirectionChange();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, _direction);

            if (_player != null && _playerVisible)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _player.position);
            }
        }

        private void Update()
        {
            CheckDirectionUpdate();
            RotateEye();
            Move();
        }

        private void FixedUpdate()
        {
            _playerVisible = Vector2.Distance(transform.position, _player.position) < maxPlayerDistance
                             && EnemyUtils.CheckEnemySeesPlayer(transform.position);
        }

        private void RotateEye()
        {
            var desiredAngle = Quaternion.Euler(0, 0, _direction.ToAngleInDegrees()).eulerAngles.z;
            var targetPos = MathfUtils.AngleToV2(desiredAngle) * magnitude;
            var nextPos = Vector2.Lerp(iris.localPosition, targetPos, eyeRotationSpeed * Time.deltaTime);
            iris.localPosition = nextPos;
        }

        private void CheckDirectionUpdate()
        {
            if (_playerVisible)
            {
                _direction = (_player.position - transform.position).normalized;
                UpdateDirectionChangeTime();
            }
            else if (Time.time > _nextDirectionChange)
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
            UpdateDirectionChangeTime();
        }

        private void UpdateDirectionChangeTime()
        {
            _nextDirectionChange = Time.time + dirResetTime;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            ChangeDirection();
        }
    }
}