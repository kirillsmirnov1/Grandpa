using System;
using DG.Tweening;
using Nightmares.Code.Control.Enemy;
using Nightmares.Code.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nightmares.Code.Control
{
    public class PlatformerFlowHandler : MonoBehaviour
    {
        public static event Action OnWin;
        public static event Action OnDefeat;
        
        [SerializeField] private CanvasGroup victoryBanner;
        [SerializeField] private CanvasGroup defeatBanner;

        private bool _gameOverTriggered;
        
        private void Awake()
        {
            Player.OnPlayerDeath += OnPlayerDeath;
            GrandpaController.OnGrandpaDefeated += OnVictory;
        }

        private void OnDestroy()
        {
            Player.OnPlayerDeath -= OnPlayerDeath;
            GrandpaController.OnGrandpaDefeated -= OnVictory;
        }

        private void OnVictory()
        {
            if(_gameOverTriggered) return;
            _gameOverTriggered = true;
            this.DelayAction(() =>
            {
                OnWin?.Invoke();
                ShowBanner(victoryBanner);
            }, 1.5f);
        }

        private void OnPlayerDeath()
        {
            if(_gameOverTriggered) return;
            _gameOverTriggered = true;
            this.DelayAction(() =>
            {
                OnDefeat?.Invoke();
                ShowBanner(defeatBanner);
            }, 1.5f);
        }

        private void ShowBanner(CanvasGroup banner)
        {
            banner.gameObject.SetActive(true);
            banner.alpha = 0;
            banner.DOFade(1f, .5f);
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}