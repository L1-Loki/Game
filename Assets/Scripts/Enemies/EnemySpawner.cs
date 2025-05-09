using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public float waveQuota;
        public float spawnInterval;
        public int spawnCount;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
    }

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnAllWavesCompleted;

    [Header("Wave Management")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private Transform player;
    private int _currentWaveIndex;

    [Header("Spawner Attributes")]
    [SerializeField] private int maxEnemiesAllowed = 20;
    [SerializeField] private float waveInterval = 10f;
    private float _spawnTimer;
    private int _enemiesAlive;
    private bool _maxEnemiesReached;

    [Header("Spawn Positions")]
    [SerializeField] public List<Transform> relativeSpawnPositions;

    private bool _isSpawning = true;
    private bool _allWavesCompleted;
    private List<GameObject> _spawnedEnemies = new List<GameObject>(); // Theo dõi kẻ thù đã sinh

    private void Awake()
    {
        // Kiểm tra các thành phần cần thiết trước khi khởi động
        if (!ValidateComponents())
        {
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (!_isSpawning || _allWavesCompleted) return;

        Wave currentWave = waves[_currentWaveIndex];
        if (currentWave.spawnCount >= currentWave.waveQuota && _enemiesAlive == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= currentWave.spawnInterval)
        {
            _spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    private bool ValidateComponents()
    {
        if (player == null)
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats != null)
            {
                player = playerStats.transform;
            }
            else
            {
                Debug.LogError("Player transform not found! Please assign in Inspector.");
                return false;
            }
        }

        if (waves.Count == 0)
        {
            Debug.LogError("Waves list is empty! Add at least one wave.");
            return false;
        }

        if (relativeSpawnPositions.Count == 0)
        {
            Debug.LogError("Spawn positions list is empty! Add spawn points.");
            return false;
        }

        return true;
    }

    private void Initialize()
    {
        _currentWaveIndex = 0;
        _spawnTimer = 0f;
        _enemiesAlive = 0;
        _maxEnemiesReached = false;
        _isSpawning = true;
        _allWavesCompleted = false;
        _spawnedEnemies.Clear();

        CalculateWaveQuota();
        OnWaveStarted?.Invoke(_currentWaveIndex + 1);
    }

    private IEnumerator BeginNextWave()
    {
        OnWaveCompleted?.Invoke(_currentWaveIndex + 1);
        _isSpawning = false;

        yield return new WaitForSeconds(waveInterval);

        if (_currentWaveIndex < waves.Count - 1)
        {
            _currentWaveIndex++;
            CalculateWaveQuota();
            _isSpawning = true;
            OnWaveStarted?.Invoke(_currentWaveIndex + 1);
        }
        else
        {
            _allWavesCompleted = true;
            OnAllWavesCompleted?.Invoke();
            Debug.Log("All waves completed!");
        }
    }

    private void CalculateWaveQuota()
    {
        if (_currentWaveIndex >= waves.Count) return;

        Wave wave = waves[_currentWaveIndex];
        wave.waveQuota = 0;
        foreach (EnemyGroup group in wave.enemyGroups)
        {
            wave.waveQuota += group.enemyCount;
        }
    }

    private void SpawnEnemy()
    {
        if (_currentWaveIndex >= waves.Count || !_isSpawning) return;

        Wave currentWave = waves[_currentWaveIndex];
        if (currentWave.spawnCount >= currentWave.waveQuota || _maxEnemiesReached) return;

        if (_enemiesAlive >= maxEnemiesAllowed)
        {
            _maxEnemiesReached = true;
            return;
        }

        List<EnemyGroup> availableGroups = currentWave.enemyGroups.FindAll(group => group.spawnCount < group.enemyCount);
        if (availableGroups.Count == 0) return;

        EnemyGroup selectedGroup = availableGroups[UnityEngine.Random.Range(0, availableGroups.Count)];
        Vector3 spawnPosition = player.position + relativeSpawnPositions[UnityEngine.Random.Range(0, relativeSpawnPositions.Count)].localPosition;

        GameObject enemy = Instantiate(selectedGroup.enemyPrefab, spawnPosition, Quaternion.identity);
        _spawnedEnemies.Add(enemy); 

        selectedGroup.spawnCount++;
        currentWave.spawnCount++;
        _enemiesAlive++;
        _maxEnemiesReached = _enemiesAlive >= maxEnemiesAllowed;
    }

    public void OnEnemyKilled()
    {
        if (_enemiesAlive > 0)
        {
            _enemiesAlive--;
            _maxEnemiesReached = false; // Cho phép sinh thêm khi có kẻ thù chết
        }
    }

    public void ResetSpawner()
    {
        StopAllCoroutines();

        for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (_spawnedEnemies[i] != null)
            {
                Destroy(_spawnedEnemies[i]);
            }
            _spawnedEnemies.RemoveAt(i);
        }
        _spawnedEnemies.Clear();


        // Reset trạng thái
        _currentWaveIndex = 0;
        _spawnTimer = 0f;
        _enemiesAlive = 0;
        _maxEnemiesReached = false;
        _isSpawning = true;
        _allWavesCompleted = false;

        // Reset spawnCount
        foreach (Wave wave in waves)
        {
            wave.spawnCount = 0;
            foreach (EnemyGroup group in wave.enemyGroups)
            {
                group.spawnCount = 0;
            }
        }

        CalculateWaveQuota();
        OnWaveStarted?.Invoke(_currentWaveIndex + 1);
        Debug.Log("EnemySpawner reset completed.");
    }

    public void PauseSpawning(bool pause)
    {
        _isSpawning = !pause;
    }

    public int CurrentWaveIndex => _currentWaveIndex;
    public int EnemiesAlive => _enemiesAlive;
    public bool AllWavesCompleted => _allWavesCompleted;
}