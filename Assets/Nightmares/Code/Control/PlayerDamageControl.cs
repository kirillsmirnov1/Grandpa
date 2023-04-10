using System;
using Nightmares.Code.Model;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class PlayerDamageControl : MonoBehaviour
    {
        public static event Action<Vector3> OnEnemyDestroyed;
        public static event Action<Vector3> OnPlayerDamaged; 

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var other = collision.gameObject;
            if (other.layer == Constants.LayerEnemy)
            {
                if (Vector3.Dot(Vector3.down, (collision.transform.position - transform.position).normalized) > 0)
                {
                    OnEnemyDestroyed?.Invoke(other.transform.position);
                    Destroy(other);    
                }
                else
                {
                    OnPlayerDamaged?.Invoke(transform.position);
                    // TODO damage player
                }
            }
        }
    }
}