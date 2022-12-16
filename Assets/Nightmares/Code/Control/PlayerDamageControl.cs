using System;
using Nightmares.Code.Model;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class PlayerDamageControl : MonoBehaviour
    {
        public static event Action<Vector3> OnEnemyDestroyed;
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            var other = collision.gameObject;
            if (other.layer == Constants.LayerEnemy)
            {
                // TODO check collision direction
                // TODO if enemy below
                    // TODO destroy enemy
                    // TODO play particles
                // TODO else
                    // TODO damage player
                    // TODO player damage fx? 
                OnEnemyDestroyed?.Invoke(other.transform.position);
                Destroy(other);
            }
        }
    }
}