using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FirstGameProg2Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("References")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text difficultyText;
        private Player player;

        [Header("Resource Settings")]
        [SerializeField] private List<SerializableDictKeyValue<ResourceEntity, int>> gameResources;
        [SerializeField] private int maxResources = 100;
        [SerializeField] private float resourceSpawnInterval = 2.5f;
        [SerializeField] private float resourceSpawnSkewConstant = 1.5f;
        private List<ResourceEntity> currentResources = new List<ResourceEntity>();
        private float resourceSpawnTimer;

        [Header("Enemy Settings")]
        [SerializeField] private List<SerializableDictKeyValue<Enemy, int>> gameEnemies;
        [SerializeField] private int maxEnemies = 100;
        [SerializeField] private float enemySpawnSkewConstant = 1.5f;
        [SerializeField] private float graceDuration = 20f;
        [SerializeField] private Vector2 initialEnemySpawnIntervalRange = new Vector2(1f, 5f);
        private Vector2 scaledEnemySpawnIntervalRange;
        private List<Enemy> currentEnemies = new List<Enemy>();
        private float enemySpawnInterval;
        private float enemySpawnTimer;

        [field: Header("Settings")]
        [field: SerializeField] public int Score { get; private set; }
        [field: SerializeField] public float Difficulty { get; private set; }
        [SerializeField] private float baseDifficulty = 1f;
        [SerializeField] private float maxDifficulty = 5f;
        [SerializeField] private float difficultyGrowthRate = 0.1f;
        [SerializeField] private float difficultyMidPoint = 90f;
        [SerializeField] private float gameTimer;

        public GameState GameState { get; private set; }
        [HideInInspector] public UnityEvent<GameState> OnGameStateChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            player = FindObjectOfType<Player>();
        }

        private void Start()
        {
            SpawnInitialResources();

            enemySpawnInterval = Random.Range(initialEnemySpawnIntervalRange.x, initialEnemySpawnIntervalRange.y);
        }

        private void Update()
        {
            UpdateGameState();

            HandleDifficulty();
            HandleScore();
        }

        private void UpdateGameState()
        {
            switch (GameState)
            {
                case GameState.PLAYING:

                    HandleResourceSpawning();
                    HandleEnemySpawning();

                    break;
                case GameState.PAUSED:



                    break;
                case GameState.UPGRADE:



                    break;
                case GameState.END:



                    break;
                default:
                    break;
            }
        }

        public void ChangeState(GameState newState)
        {
            Debug.Log($"Changing to {newState}");

            switch (newState)
            {
                case GameState.PLAYING:

                    Time.timeScale = 1f;

                    break;
                case GameState.PAUSED:

                    Time.timeScale = 0f;

                    break;
                case GameState.UPGRADE:

                    Time.timeScale = 0f;

                    break;
                case GameState.END:



                    break;
                default:
                    break;
            }

            GameState = newState;
            OnGameStateChanged?.Invoke(newState);
        }

        private void SpawnInitialResources()
        {
            for (int i = 0; i < maxResources; i++)
            {
                SpawnRandomResource();
            }
        }

        private void HandleResourceSpawning()
        {
            foreach(var resource in new List<ResourceEntity>(currentResources))
            {
                if(resource == null) currentResources.Remove(resource);
            }

            if (currentResources.Count >= maxResources) return;

            resourceSpawnTimer += Time.deltaTime;

            if(resourceSpawnTimer > resourceSpawnInterval)
            {
                resourceSpawnTimer = 0f;

                SpawnRandomResource();
            }
        }

        private void HandleEnemySpawning()
        {
            if (gameTimer < graceDuration) return;

            foreach (var enemy in new List<Enemy>(currentEnemies))
            {
                if (enemy == null) currentEnemies.Remove(enemy);
            }

            if (currentEnemies.Count >= maxEnemies) return;

            enemySpawnTimer += Time.deltaTime;

            if (enemySpawnTimer > enemySpawnInterval)
            {
                enemySpawnTimer = 0f;
                enemySpawnInterval = Random.Range(scaledEnemySpawnIntervalRange.x, scaledEnemySpawnIntervalRange.y);

                SpawnRandomEnemy();
            }
        }

        public void AddScore(int amt)
        {
            Score += amt;

            Score = Mathf.Clamp(Score, 0, int.MaxValue);
        }

        private void HandleDifficulty()
        {
            gameTimer += Time.deltaTime;

            Difficulty = baseDifficulty + (maxDifficulty-1) / (1 + Mathf.Exp(-difficultyGrowthRate * (gameTimer - difficultyMidPoint)));
            Difficulty = Mathf.Clamp(Difficulty, baseDifficulty, maxDifficulty);

            scaledEnemySpawnIntervalRange = Vector2.Lerp(initialEnemySpawnIntervalRange, new Vector2(0.5f, 2f), (Difficulty - 1) / (maxDifficulty - 1));

            difficultyText.text = $"Difficulty: {System.Math.Round(Difficulty, 3)}";
        }

        private void HandleScore()
        {
            scoreText.text = $"Score: {Score}";
        }

        private void SpawnRandomResource()
        {
            float totalWeight = 0f;
            foreach (var keyValue in gameResources)
            {
                totalWeight += 1 / Mathf.Pow(keyValue.Value, resourceSpawnSkewConstant);
            }

            float randomFloat = Random.Range(0f, totalWeight);

            Vector3 randomPosition = GetRandomPositionAwayFromPlayer();
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            foreach (var keyValue in gameResources)
            {
                float weightDifference = randomFloat - (1 / Mathf.Pow(keyValue.Value, resourceSpawnSkewConstant));
                if (weightDifference >= 0)
                {
                    randomFloat = weightDifference;
                }
                else
                {
                    currentResources.Add(Instantiate(keyValue.Key, randomPosition, randomRotation, transform));
                    break;
                }
            }
        }

        private void SpawnRandomEnemy()
        {
            float totalWeight = 0f;
            foreach(var keyValue in gameEnemies)
            {
                totalWeight += 1 / Mathf.Pow(keyValue.Value, enemySpawnSkewConstant);
            }

            float randomFloat = Random.Range(0f, totalWeight);

            Vector3 randomPosition = GetRandomPositionAwayFromPlayer();
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            foreach(var keyValue in gameEnemies)
            {
                float weightDifference = randomFloat - (1 / Mathf.Pow(keyValue.Value, enemySpawnSkewConstant));
                if (weightDifference >= 0)
                {
                    randomFloat = weightDifference;
                }
                else
                {
                    Enemy spawnedEnemy = Instantiate(keyValue.Key, randomPosition, randomRotation, transform);
                    spawnedEnemy.AdjustToDifficulty();

                    currentEnemies.Add(spawnedEnemy);
                    break;
                }
            }
        }

        private Vector3 GetRandomPositionAwayFromPlayer()
        {
            Vector3 randomPosition = new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), 0);
            float radius = 5f;

            while(Vector3.Distance(randomPosition, player.transform.position) < radius){
                randomPosition = new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), 0);
            }

            return randomPosition;
        }
    }
}

public enum GameState
{
    PLAYING,
    PAUSED,
    UPGRADE,
    END
}
