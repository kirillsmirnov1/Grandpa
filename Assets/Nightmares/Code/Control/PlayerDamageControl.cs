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
                var enemy = other.GetComponent<Enemy.Enemy>();
                var dot = Vector3.Dot(Vector3.down, toEnemy);
                var angle = Mathf.Acos(dot) * 180 / Mathf.PI;
                if (angle > 45) // TODO ? Handle by Enemy as well??
                {
                    player.DoDamage();
                    if (enemy.ThrownBackByPlayerAttack)
                    {
                        enemy.rb.velocity = Vector2.zero;
                        enemy.rb.AddForce(recoilForceMod * toEnemy, ForceMode2D.Impulse);
                    }

                    ThrowbackPlayer(toEnemy);
                    OnPlayerDamaged?.Invoke(transform.position);
                }
                else if (enemy.CanBeDamaged)
                {
                    enemy.Damage();
                    if (enemy.ThrowbacksPlayerOnAttack)
                    {
                        ThrowbackPlayer(toEnemy);
                    }
                }
                else
                {
                    ThrowbackPlayer(toEnemy);
                }
            }
        }

        private void ThrowbackPlayer(Vector3 toEnemy)
        {
            player.rb.velocity = Vector2.zero;
            player.rb.AddForce(-recoilForceMod * toEnemy, ForceMode2D.Impulse);
        }
    }
}