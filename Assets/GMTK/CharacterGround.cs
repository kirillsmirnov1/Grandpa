using UnityEngine;

namespace GMTK
{
    //This script is used by both movement and jump to detect when the character is touching the ground
    public class CharacterGround : MonoBehaviour
    {
        private bool onGround;
       
        [Header("Collider Settings")]
        [Tooltip("Length of the ground-checking collider")] 
        [SerializeField] private float groundLength = 0.95f;
        [Tooltip("Distance between the ground-checking colliders")] 
        [SerializeField] private Vector3 colliderOffset;

        [Header("Layer Masks")]
        [Tooltip("Which layers are read as the ground")] 
        [SerializeField] private LayerMask groundLayer;
        
        private void Update()
        {
            //Determine if the player is stood on objects on the ground layer, using a pair of raycasts
            var position = transform.position;
            onGround = Physics2D.Raycast(position + colliderOffset, Vector2.down, groundLength, groundLayer) 
                       || Physics2D.Raycast(position - colliderOffset, Vector2.down, groundLength, groundLayer);
        }

        private void OnDrawGizmos()
        {
            //Draw the ground colliders on screen for debug purposes
            Gizmos.color = onGround ? Color.green : Color.red;

            var position = transform.position;
            
            Gizmos.DrawLine(position + colliderOffset, position + colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine(position - colliderOffset, position - colliderOffset + Vector3.down * groundLength);
        }

        //Send ground detection to other scripts
        public bool GetOnGround() { return onGround; }
    }
}

