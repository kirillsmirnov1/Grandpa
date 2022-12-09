using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control
{
    public class CameraFollowsPlayer : MonoBehaviour
    {
        public float lerpSpeed = 1f;
        
        public Transform target;
        public TilemapRenderer baseTiles;

        private Camera _cam;
        
        private float _yMax;
        private float _yMin;
        
        // TODO move with lerp

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            CalculateLimitsFromTileMap(baseTiles);
        }

        private void FixedUpdate()
        {
            FollowPlayer(Time.fixedDeltaTime);
        }

        private void FollowPlayer(float dt)
        {
            var pos = transform.position;
            var newY = Mathf.Clamp(target.position.y, _yMin + _cam.orthographicSize, _yMax - _cam.orthographicSize);
            pos.y = newY;
            transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed * dt);
        }

        private void CalculateLimitsFromTileMap(TilemapRenderer tiles)
        {
            _yMax = tiles.bounds.max.y;
            _yMin = tiles.bounds.min.y;
        }
    }
}