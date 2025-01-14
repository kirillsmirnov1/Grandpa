﻿using System;
using System.Collections;
using DG.Tweening;
using Nightmares.Code.Audio;
using Nightmares.Code.Control.Enemy;
using Nightmares.Code.Extensions;
using Nightmares.Code.Model;
using Nightmares.Code.Model.Quests;
using Nightmares.Code.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
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
        [SerializeField] private Tilemap bossTilemap;
        [SerializeField] private EnemySpawn enemySpawn;
        [SerializeField] private GameOverBanner gameOverBanner;
        [SerializeField] private CameraFollowsPlayer cameraFollowsPlayer;
        [SerializeField] private Player player;
        [SerializeField] private RectTransform mobileInputs;
        [SerializeField] private PointsCounter pointsCounter;
        [SerializeField] private StartBanner startBanner;

        [Header("Data")]
        [SerializeField] private QuestManager questManager;
        [SerializeField] private IntVariable currentDifficulty;
        [SerializeField] private IntVariable maxUnlockedDifficulty;
        [SerializeField] private StringArrayVariable questsCompleted;
        
        private bool _gameOverTriggered;

        public Vector2Int LevelDimensions => levelDimensions;
        public int GrandpasRoomHeight => grandpasRoomHeight;
        public int StoriesUnlocked => Mathf.Min(questsCompleted.Length * 3, 15);
#if UNITY_EDITOR
        public int Difficulty => debugDifficulty == 0 ? currentDifficulty.Value : debugDifficulty;
#else
        public int Difficulty => currentDifficulty.Value;
#endif
        
        private void Awake()
        {
            Instance = this;
            
            Player.OnPlayerDeath += OnPlayerDeath;
            GrandpaController.OnGrandpaDefeated += OnVictory;
        }

        private void Start()
        {
            questManager.PrepareForSession();
            startBanner.Show(() => StartCoroutine(StartGame()));
            
            AudioManager.Instance.PlayIdleMusic();
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
                    new Vector3((topPlatform.x + topPlatform.y) / 2f + .5f, 0);
            }

            player.gameObject.SetActive(true);

            grandpaWrap.position = new Vector3(0, -levelDimensions.y);
            gridGeneration.MergeIn(bossTilemap);
            grandpaWrap.gameObject.SetActive(true);

            yield return null;

            enemySpawn.SpawnEnemies();

            LayoutRebuilder.ForceRebuildLayoutImmediate(mobileInputs);
            
            AudioManager.Instance.PlayActionMusic();
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

            questManager.SaveCompletedQuests();
            var completedQuests = questManager.CompletedInSession;

            this.DelayAction(() =>
            {
                callback?.Invoke();
                ShowBanner(gameOverBanner.GetComponent<CanvasGroup>());
                gameOverBanner.Set(victory, pointsCounter.Points, Difficulty, completedQuests);
                AudioManager.Instance.PlayIdleMusic();
                
                var player = Player.Instance;

                cameraFollowsPlayer.enabled = false;
                player.gameObject.SetActive(false);
                player.transform.position = new Vector3(0, 100);

                if (victory)
                {
                    var nextDiff = Mathf.Min(currentDifficulty.Value + 1, Constants.GrandpaMaxDifficulty);
                    maxUnlockedDifficulty.Value = Mathf.Max(nextDiff, maxUnlockedDifficulty);
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