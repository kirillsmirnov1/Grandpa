using Nightmares.Code.Model;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class PlayerDamageControl : MonoBehaviour
    {
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
                Destroy(other);
            }
        }
    }
}