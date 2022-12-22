using Nightmares.Code.Model;
using UnityEngine;

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

            if (_player != null)
            {
                Gizmos.color = _playerVisible ? Color.green : Color.red;
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
            CheckPlayerVisible();
        }

        private void CheckPlayerVisible()
        {
            var visible = true;
            var pos = transform.position;
            var toPlayer = _player.position - pos;

            var hits = Physics2D.RaycastAll(pos, toPlayer, toPlayer.magnitude);
            foreach (var hit in hits)
            {
                var layer = hit.transform.gameObject.layer;
                if (layer is Constants.LayerEnemy or Constants.LayerPlayer)
                {
                    continue;
                }

                visible = false;
                break;
            }

            _playerVisible = visible;
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