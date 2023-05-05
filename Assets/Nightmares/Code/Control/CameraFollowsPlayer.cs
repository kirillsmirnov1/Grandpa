using UnityEngine;

namespace Nightmares.Code.Control
{
    public class CameraFollowsPlayer : MonoBehaviour
    {
        public float lerpSpeed = 1f;
        
        public Transform target;
        [SerializeField] private PlatformerGameManager gameManager;

        private Camera _cam;
        
        private float _yMax;
        private float _yMin;
        
        // TODO move with lerp

        private void Awake()
        {
            _cam = GetComponent<Camera>();
        }

        private void FixedUpdate()
        {
            FollowPlayer(Time.fixedDeltaTime);
        }

        public void InitDimensions()
        {
            _yMax = 1f;
            _yMin = -gameManager.LevelDimensions.y - gameManager.GrandpasRoomHeight;
        }

        private void FollowPlayer(float dt)
        {
            var pos = transform.position;
            var newY = Mathf.Clamp(target.position.y, _yMin + _cam.orthographicSize, _yMax - _cam.orthographicSize);
            pos.y = newY;
            transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed * dt);
        }
    }
}