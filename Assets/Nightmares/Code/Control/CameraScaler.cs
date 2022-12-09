using UnityEngine;

namespace Nightmares.Code.Control
{
    public class CameraScaler : MonoBehaviour
    {
        public float desiredWidth = 14;

        private Camera _cam;
        
        private void Awake()
        {
            _cam = GetComponent<Camera>();
            RescaleCamera();
        }

        private void RescaleCamera()
        {
            var initialSize = _cam.orthographicSize;
            var initialWidth = 2f * initialSize * _cam.aspect;
            _cam.orthographicSize = initialSize * desiredWidth / initialWidth;
        }
    }
}
