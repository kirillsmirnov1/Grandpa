using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control
{
    public class CameraFollowsPlayer : MonoBehaviour
    {
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

        private void Update()
        {
            FollowPlayer();
        }

        private void FollowPlayer()
        {
            var pos = transform.position;
            var newY = Mathf.Clamp(target.position.y, _yMin + _cam.orthographicSize, _yMax - _cam.orthographicSize);
            pos.y = newY;
            transform.position = pos;
        }

        private void CalculateLimitsFromTileMap(TilemapRenderer tiles)
        {
            _yMax = tiles.bounds.max.y;
            _yMin = tiles.bounds.min.y;
        }
    }
}