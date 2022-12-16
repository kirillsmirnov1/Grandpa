using UnityEngine;

namespace Nightmares.Code.Control
{
    public class Particles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem enemyDeathParticles;

        private void Awake()
        {
            PlayerDamageControl.OnEnemyDestroyed += PlayEnemyDeathParticles;
        }
        
        private void OnDestroy()
        {
            PlayerDamageControl.OnEnemyDestroyed -= PlayEnemyDeathParticles;
        }

        private void PlayEnemyDeathParticles(Vector3 pos)
        {
            enemyDeathParticles.transform.position = pos;
            enemyDeathParticles.Play();
        }
    }
}