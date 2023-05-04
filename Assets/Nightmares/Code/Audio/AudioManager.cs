using Nightmares.Code.Control;
using Nightmares.Code.Control.Enemy;
using UnityEngine;

namespace Nightmares.Code.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] enemyHitClip;
        [SerializeField] private AudioClip playerHitSound;
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip defeatSound;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource musicSource;
        
        private void Awake()
        {
            Enemy.OnEnemyDamaged += PlayEnemyDamagedSound;
            PlatformerGameManager.OnWin += OnVictory;
            PlatformerGameManager.OnDefeat += OnDefeat;
            Player.OnPlayerDamage += OnPlayerDamage;
        }

        private void OnDestroy()
        {
            Enemy.OnEnemyDamaged -= PlayEnemyDamagedSound;
            PlatformerGameManager.OnWin -= OnVictory;
            PlatformerGameManager.OnDefeat -= OnDefeat;
            Player.OnPlayerDamage -= OnPlayerDamage;
        }

        private void OnPlayerDamage()
        {
            sfxAudioSource.PlayOneShot(playerHitSound);
        }

        private void OnDefeat()
        {
            musicSource.Stop();
            sfxAudioSource.PlayOneShot(defeatSound);
        }

        private void OnVictory()
        {
            musicSource.Stop();
            sfxAudioSource.PlayOneShot(winSound);
        }

        private void PlayEnemyDamagedSound()
        {
            sfxAudioSource.PlayOneShot(enemyHitClip[Random.Range(0, enemyHitClip.Length)]);
        }
    }
}
