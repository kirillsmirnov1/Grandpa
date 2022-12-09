using UnityEngine;

namespace Nightmares.Code.Control
{
    public class CameraFollowsPlayer : MonoBehaviour
    {
        public Transform target;

        // TODO move with lerp
        // TODO limits 
        
        private void Update()
        {
            var pos = transform.position;
            pos.y = target.position.y;
            transform.position = pos;
        }
    }
}