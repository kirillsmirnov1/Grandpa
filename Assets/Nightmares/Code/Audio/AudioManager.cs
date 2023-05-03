using Nightmares.Code.Control.Enemy;
using UnityEngine;

namespace Nightmares.Code.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] enemyHitClip;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
            Enemy.OnEnemyDamaged += PlayEnemyDamagedSound;
        }

        private void OnDestroy()
        {
            Enemy.OnEnemyDamaged -= PlayEnemyDamagedSound;
        }

        private void PlayEnemyDamagedSound()
        {
            _audioSource.PlayOneShot(enemyHitClip[Random.Range(0, enemyHitClip.Length)]);
        }
    }
}
