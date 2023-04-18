using System;
using Nightmares.Code.Model;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class PlayerDamageControl : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private float recoilForceMod = 1f;
        
        public static event Action<Vector3> OnPlayerDamaged; 

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var other = collision.gameObject;
            if (other.layer == Constants.LayerEnemy)
            {
                var toEnemy = (collision.transform.position - transform.position).normalized;
                var enemy = other.GetComponent<Enemy>();
                if (Vector3.Dot(Vector3.down, toEnemy) > 0) // TODO ? Handle by Enemy as well??
                {
                    enemy.Damage();  
                }
                else
                {
                    player.DoDamage();
                    enemy.rb.AddForce(recoilForceMod * toEnemy, ForceMode2D.Impulse); // TODO should be handled by Enemy 
                    player.rb.AddForce(-recoilForceMod * toEnemy, ForceMode2D.Impulse);
                    OnPlayerDamaged?.Invoke(transform.position);
                }
            }
        }
    }
}