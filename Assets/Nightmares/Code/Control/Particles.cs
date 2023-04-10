using UnityEngine;

namespace Nightmares.Code.Control
{
    public class Particles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem enemyDeathParticles;
        [SerializeField] private ParticleSystem playerDamageParticles;

        private void Awake()
        {
            PlayerDamageControl.OnEnemyDestroyed += PlayEnemyDeathParticles;
            PlayerDamageControl.OnPlayerDamaged += PlayPlayerDamageParticles;
        }
        
        private void OnDestroy()
        {
            PlayerDamageControl.OnEnemyDestroyed -= PlayEnemyDeathParticles;
            PlayerDamageControl.OnPlayerDamaged -= PlayPlayerDamageParticles;
        }

        private void PlayPlayerDamageParticles(Vector3 pos)
        {
            PlayParticlesAtPos(playerDamageParticles, pos);
        }

        private void PlayEnemyDeathParticles(Vector3 pos)
        {
            PlayParticlesAtPos(enemyDeathParticles, pos);
        }

        private void PlayParticlesAtPos(ParticleSystem particles, Vector3 pos)
        {
            particles.transform.position = pos;
            particles.Play();
        }
    }
}