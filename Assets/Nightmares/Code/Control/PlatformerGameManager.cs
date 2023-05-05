using System;
using System.Collections;
using DG.Tweening;
using Nightmares.Code.Control.Enemy;
using Nightmares.Code.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nightmares.Code.Control
{
    public class PlatformerGameManager : MonoBehaviour
    {
        public static event Action OnWin;
        public static event Action OnDefeat;
     
        [SerializeField] private Vector2Int levelDimensions = new(13, 40);
        [SerializeField] private int grandpasRoomHeight = 8;
        [SerializeField] private GridGeneration gridGeneration;
        [SerializeField] private Transform grandpaWrap;
        [SerializeField] private EnemySpawn enemySpawn;
        [SerializeField] private CanvasGroup victoryBanner;
        [SerializeField] private CanvasGroup defeatBanner;
        [SerializeField] private CameraFollowsPlayer cameraFollowsPlayer;

        private bool _gameOverTriggered;

        public Vector2Int LevelDimensions => levelDimensions;
        public int GrandpasRoomHeight => grandpasRoomHeight;
        
        private void Awake()
        {
            Player.OnPlayerDeath += OnPlayerDeath;
            GrandpaController.OnGrandpaDefeated += OnVictory;
        }

        private IEnumerator Start()
        {
            gridGeneration.SpawnTileMap();
            grandpaWrap.position = new Vector3(0, -levelDimensions.y);
            grandpaWrap.gameObject.SetActive(true);
            yield return null;
            enemySpawn.SpawnEnemies();
        }

        private void OnDestroy()
        {
            Player.OnPlayerDeath -= OnPlayerDeath;
            GrandpaController.OnGrandpaDefeated -= OnVictory;
        }

        private void OnVictory()
        {
            HandleGameOver(victoryBanner, OnWin);
        }

        private void OnPlayerDeath()
        {
            HandleGameOver(defeatBanner, OnDefeat);
        }

        private void HandleGameOver(CanvasGroup bannerCG, Action callback)
        {
            if(_gameOverTriggered) return;
            _gameOverTriggered = true;
            this.DelayAction(() =>
            {
                callback?.Invoke();
                ShowBanner(bannerCG);
                
                var player = Player.Instance;

                cameraFollowsPlayer.enabled = false;
                player.gameObject.SetActive(false);
                player.transform.position = new Vector3(0, 100);
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