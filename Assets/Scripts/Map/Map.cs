using System;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("Map Generation Settings")]
    [SerializeField] private List<GameObject> terrainChunks = new List<GameObject>();
    [SerializeField] private GameObject player;
    [SerializeField] private float checkerRadius = 0.5f;
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private float chunkSize = 20f; // Kích thước chunk để tính toán khoảng cách

    [Header("Optimization Settings")]
    [SerializeField] private float maxOptimizationDistance = 40f;
    [SerializeField] private float optimizationCooldownDuration = 0.5f;
    [SerializeField] private float boundaryForce = 100f; // Lực đẩy người chơi khỏi biên

    private readonly Dictionary<Vector2Int, GameObject> _spawnedChunks = new Dictionary<Vector2Int, GameObject>();
    public GameObject _currentChunk;
    private Player _player;
    private float _optimizationCooldown;
    private Vector2Int _lastChunkCoord;

    private readonly Vector2Int[] _directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),   // Right
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(0, 1),   // Up
        new Vector2Int(0, -1),  // Down
        new Vector2Int(1, 1),   // Right Up
        new Vector2Int(1, -1),  // Right Down
        new Vector2Int(-1, 1),  // Left Up
        new Vector2Int(-1, -1)  // Left Down
    };

    private void Start()
    {
        Initialize();
        SpawnInitialChunks();
    }

    private void Update()
    {
        UpdateCurrentChunk();
        CheckAndSpawnChunks();
        OptimizeChunks();
        PreventPlayerOutOfBounds();
    }

    private void Initialize()
    {
        _player = FindObjectOfType<Player>();
        if (_player == null || terrainChunks.Count == 0 || player == null)
        {
            Debug.LogError("Initialization failed: Missing required components!");
            enabled = false;
        }
    }

    private void SpawnInitialChunks()
    {
        Vector2Int startCoord = Vector2Int.zero;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int coord = new Vector2Int(i, j);
                SpawnChunkAtCoord(coord);
            }
        }
        _lastChunkCoord = startCoord;
    }

    private void UpdateCurrentChunk()
    {
        Vector2 playerPos = player.transform.position;
        Vector2Int currentCoord = WorldToChunkCoord(playerPos);

        if (currentCoord != _lastChunkCoord && _spawnedChunks.ContainsKey(currentCoord))
        {
            _currentChunk = _spawnedChunks[currentCoord];
            _lastChunkCoord = currentCoord;
        }
    }

    private void CheckAndSpawnChunks()
    {
        Vector2Int playerChunkCoord = WorldToChunkCoord(player.transform.position);

        // Sử dụng khoảng cách Euclidean để kiểm tra các chunk lân cận
        foreach (Vector2Int dir in _directions)
        {
            Vector2Int checkCoord = playerChunkCoord + dir;
            if (!_spawnedChunks.ContainsKey(checkCoord))
            {
                float distance = Vector2Int.Distance(playerChunkCoord, checkCoord);
                if (distance <= 1.5f) // Chỉ sinh chunk trong phạm vi 1.5 đơn vị
                {
                    SpawnChunkAtCoord(checkCoord);
                }
            }
        }
    }

    private void SpawnChunkAtCoord(Vector2Int coord)
    {
        if (terrainChunks.Count == 0) return;

        Vector3 position = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
        int randomIndex = UnityEngine.Random.Range(0, terrainChunks.Count);
        GameObject newChunk = Instantiate(terrainChunks[randomIndex], position, Quaternion.identity);
        _spawnedChunks[coord] = newChunk;
    }

    private void OptimizeChunks()
    {
        _optimizationCooldown -= Time.deltaTime;
        if (_optimizationCooldown > 0f) return;

        _optimizationCooldown = optimizationCooldownDuration;
        Vector2 playerPos = player.transform.position;
        Vector2Int playerCoord = WorldToChunkCoord(playerPos);

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var chunk in _spawnedChunks)
        {
            float distance = Vector2Int.Distance(playerCoord, chunk.Key) * chunkSize;
            bool shouldBeActive = distance <= maxOptimizationDistance;
            chunk.Value.SetActive(shouldBeActive);

            if (!shouldBeActive && distance > maxOptimizationDistance * 2)
            {
                toRemove.Add(chunk.Key);
            }
        }

        foreach (var coord in toRemove)
        {
            Destroy(_spawnedChunks[coord]);
            _spawnedChunks.Remove(coord);
        }
    }

    private void PreventPlayerOutOfBounds()
    {
        Vector2 playerPos = player.transform.position;
        Vector2Int playerCoord = WorldToChunkCoord(playerPos);

        // Tính toán biên giới dựa trên các chunk hiện có
        Vector2 minBounds = Vector2.positiveInfinity;
        Vector2 maxBounds = Vector2.negativeInfinity;

        foreach (var chunkCoord in _spawnedChunks.Keys)
        {
            maxBounds = Vector2.Max(maxBounds, (Vector2)chunkCoord * chunkSize);
            maxBounds = Vector2.Max(maxBounds, (Vector2)(chunkCoord + Vector2Int.one) * chunkSize);
        }

        // Áp dụng lực đẩy nếu người chơi gần biên
        Vector2 force = Vector2.zero;
        float boundaryBuffer = chunkSize * 0.2f;

        if (playerPos.x < minBounds.x + boundaryBuffer)
            force.x = boundaryForce;
        else if (playerPos.x > maxBounds.x - boundaryBuffer)
            force.x = -boundaryForce;

        if (playerPos.y < minBounds.y + boundaryBuffer)
            force.y = boundaryForce;
        else if (playerPos.y > maxBounds.y - boundaryBuffer)
            force.y = -boundaryForce;

        if (force != Vector2.zero)
        {
            _player.GetComponent<Rigidbody2D>().AddForce(force * Time.deltaTime, ForceMode2D.Impulse);
        }
    }

    private Vector2Int WorldToChunkCoord(Vector2 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / chunkSize),
            Mathf.FloorToInt(worldPos.y / chunkSize)
        );
    }
}