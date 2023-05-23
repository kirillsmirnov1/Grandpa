using System;
using System.Collections;
using DG.Tweening;
using Nightmares.Code.Control.Enemy;
using Nightmares.Code.Extensions;
using Nightmares.Code.Model;
using Nightmares.Code.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityUtils.Variables;

namespace Nightmares.Code.Control
{
    public class PlatformerGameManager : MonoBehaviour
    {
        public static event Action OnWin;
        public static event Action OnDefeat;

        public static PlatformerGameManager Instance;

#if UNITY_EDITOR
        [Range(0, Constants.GrandpaMaxDifficulty)]
        [SerializeField] private int debugDifficulty = 0;
#endif

        [Header("Level dimensions")]
        [SerializeField] private int yHeightPerDifficultyLevel = 50;
        [SerializeField] private Vector2Int levelDimensions = new(13, 40);
        [SerializeField] private int grandpasRoomHeight = 8;
        
        [Header("Components")]
        [SerializeField] private GridGeneration gridGeneration;
        [SerializeField] private Transform grandpaWrap;
        [SerializeField] private EnemySpawn enemySpawn;
        [SerializeField] private GameOverBanner gameOverBanner;
        [SerializeField] private CameraFollowsPlayer cameraFollowsPlayer;
        [SerializeField] private Player player;
        [SerializeField] private RectTransform mobileInputs;
        [SerializeField] private PointsCounter pointsCounter;
        [SerializeField] private StartBanner startBanner;

        [Header("Data")]
        [SerializeField] private IntVariable currentDifficulty;
        [SerializeField] private IntVariable maxUnlockedDifficulty;
        
        private bool _gameOverTriggered;

        public Vector2Int LevelDimensions => levelDimensions;
        public int GrandpasRoomHeight => grandpasRoomHeight;
#if UNITY_EDITOR
        public int Difficulty => debugDifficulty == 0 ? currentDifficulty.Value : debugDifficulty;
#else
        public int Difficulty => Prefs.GrandpaDifficulty;
#endif
        
        private void Awake()
        {
            Instance = this;
            
            Player.OnPlayerDeath += OnPlayerDeath;
            GrandpaController.OnGrandpaDefeated += OnVictory;
        }

        private void Start()
        {
            startBanner.Show(() => StartCoroutine(StartGame()));
        }

        private IEnumerator StartGame()
        {
            levelDimensions.y += (Difficulty - 1) * yHeightPerDifficultyLevel;
            cameraFollowsPlayer.InitDimensions();
            yield return null;

            gridGeneration.SpawnTileMap(out var impl);

            yield return impl;

            if (gridGeneration.platforms.Count > 0)
            {
                var topPlatform = gridGeneration.platforms[0];
                player.transform.position =
                    new Vector3((topPlatform.x + topPlatform.y) / 2f, topPlatform.y + 1);
            }

            player.gameObject.SetActive(true);

            grandpaWrap.position = new Vector3(0, -levelDimensions.y);
            grandpaWrap.gameObject.SetActive(true);

            yield return null;

            enemySpawn.SpawnEnemies();

            LayoutRebuilder.ForceRebuildLayoutImmediate(mobileInputs);
        }

        private void Update()
        {
            if (_gameOverTriggered && Input.GetKeyDown(KeyCode.R))
            {
                ReloadScene();
            }
        }

        private void OnDestroy()
        {
            Player.OnPlayerDeath -= OnPlayerDeath;
            GrandpaController.OnGrandpaDefeated -= OnVictory;
        }

        private void OnVictory()
        {
            HandleGameOver(true, OnWin);
        }

        private void OnPlayerDeath()
        {
            HandleGameOver(false, OnDefeat);
        }

        private void HandleGameOver(bool victory, Action callback)
        {
            if(_gameOverTriggered) return;
            _gameOverTriggered = true;
            this.DelayAction(() =>
            {
                callback?.Invoke();
                ShowBanner(gameOverBanner.GetComponent<CanvasGroup>());
                gameOverBanner.Set(victory, pointsCounter.Points, Difficulty);
                
                var player = Player.Instance;

                cameraFollowsPlayer.enabled = false;
                player.gameObject.SetActive(false);
                player.transform.position = new Vector3(0, 100);

                if (victory)
                {
                    maxUnlockedDifficulty.Value = Mathf.Min(maxUnlockedDifficulty.Value + 1, Constants.GrandpaMaxDifficulty);
                }
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