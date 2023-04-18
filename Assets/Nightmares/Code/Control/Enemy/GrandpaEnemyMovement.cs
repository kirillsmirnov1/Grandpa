using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class GrandpaEnemyMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 1f;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Rigidbody2D staffRb;
    }
}