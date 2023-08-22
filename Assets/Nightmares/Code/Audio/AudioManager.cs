using DG.Tweening;
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
        [SerializeField] private AudioSource idleMusic;
        [SerializeField] private AudioSource actionMusic;

        private AudioSource _lastActiveMusicSource;
        
        public static AudioManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
            
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

        public void PlayActionMusic()
        {
            SwitchMusicSources(_lastActiveMusicSource, actionMusic, .4f);
            _lastActiveMusicSource = actionMusic;
        }

        public void PlayIdleMusic()
        {
            SwitchMusicSources(_lastActiveMusicSource, idleMusic, 1f);
            _lastActiveMusicSource = idleMusic;
        }

        private void SwitchMusicSources(AudioSource from, AudioSource to, float targetVolume)
        {
            from?.DOFade(0, .5f);
            to?.DOFade(targetVolume, .5f);
        }

        private void OnPlayerDamage()
        {
            sfxAudioSource.PlayOneShot(playerHitSound);
        }

        private void OnDefeat()
        {
            sfxAudioSource.PlayOneShot(defeatSound);
        }

        private void OnVictory()
        {
            sfxAudioSource.PlayOneShot(winSound);
        }

        private void PlayEnemyDamagedSound()
        {
            sfxAudioSource.PlayOneShot(enemyHitClip[Random.Range(0, enemyHitClip.Length)]);
        }
    }
}
