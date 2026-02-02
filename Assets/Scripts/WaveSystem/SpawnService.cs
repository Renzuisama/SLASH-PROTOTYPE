using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles actual enemy spawning with validation, pooling, and positioning
/// </summary>
[RequireComponent(typeof(EnemyPool))]
public class SpawnService : MonoBehaviour
{
    [Header("Spawn Validation")]
    [Tooltip("Minimum distance from player to spawn")]
    [Min(0.5f)]
    public float minDistanceFromPlayer = 2f;

    [Tooltip("Layer mask for obstacles that block spawning")]
    public LayerMask obstacleLayer;

    [Tooltip("Radius for obstacle detection")]
    [Min(0.1f)]
    public float obstacleCheckRadius = 0.5f;

    [Tooltip("Max attempts to find valid spawn position")]
    [Min(1)]
    public int maxSpawnAttempts = 10;

    [Header("References")]
    [Tooltip("Player transform (auto-found if null)")]
    public Transform player;

    private EnemyPool pool;
    private int totalSpawned = 0;
    private int totalKilled = 0;

    public int TotalSpawned => totalSpawned;
    public int TotalKilled => totalKilled;

    private void Awake()
    {
        pool = GetComponent<EnemyPool>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void OnEnable()
    {
        EnemyDeathBroadcaster.OnEnemyDied += OnEnemyDied;
    }

    private void OnDisable()
    {
        EnemyDeathBroadcaster.OnEnemyDied -= OnEnemyDied;
    }

    /// <summary>
    /// Attempt to spawn an enemy at a valid position
    /// </summary>
    public GameObject SpawnEnemy(
        GameObject prefab,
        float spawnRadiusMin,
        float spawnRadiusMax,
        Vector2 worldBoundsMin,
        Vector2 worldBoundsMax,
        bool useWorldBounds)
    {
        if (prefab == null)
        {
            Debug.LogWarning("SpawnService: Cannot spawn null prefab");
            return null;
        }

        if (player == null)
        {
            Debug.LogWarning("SpawnService: No player reference");
            return null;
        }

        Vector3 spawnPosition;
        bool foundValidPosition = false;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            spawnPosition = GetRandomRingPosition(
                player.position,
                spawnRadiusMin,
                spawnRadiusMax,
                worldBoundsMin,
                worldBoundsMax,
                useWorldBounds
            );

            if (IsValidSpawnPosition(spawnPosition))
            {
                foundValidPosition = true;
                GameObject enemy = pool.Get(prefab, spawnPosition, Quaternion.identity);

                if (enemy != null)
                {
                    totalSpawned++;

                    // Ensure EnemyDeathBroadcaster exists
                    if (enemy.GetComponent<EnemyDeathBroadcaster>() == null)
                    {
                        enemy.AddComponent<EnemyDeathBroadcaster>();
                    }

                    return enemy;
                }
                break;
            }
        }

        if (!foundValidPosition)
        {
            Debug.LogWarning($"SpawnService: Could not find valid spawn position after {maxSpawnAttempts} attempts");
        }

        return null;
    }

    /// <summary>
    /// Get random position in ring around player
    /// </summary>
    private Vector3 GetRandomRingPosition(
        Vector3 center,
        float radiusMin,
        float radiusMax,
        Vector2 boundsMin,
        Vector2 boundsMax,
        bool useBounds)
    {
        // Random angle
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Random radius in ring
        float radius = Random.Range(radiusMin, radiusMax);

        // Calculate position
        float x = center.x + Mathf.Cos(angle) * radius;
        float y = center.y + Mathf.Sin(angle) * radius;

        // Clamp to world bounds if enabled
        if (useBounds)
        {
            x = Mathf.Clamp(x, boundsMin.x, boundsMax.x);
            y = Mathf.Clamp(y, boundsMin.y, boundsMax.y);
        }

        return new Vector3(x, y, 0f);
    }

    /// <summary>
    /// Check if position is valid for spawning
    /// </summary>
    private bool IsValidSpawnPosition(Vector3 position)
    {
        if (player == null) return false;

        // Check distance from player
        float distanceToPlayer = Vector2.Distance(
            new Vector2(position.x, position.y),
            new Vector2(player.position.x, player.position.y)
        );

        if (distanceToPlayer < minDistanceFromPlayer)
        {
            return false;
        }

        // Check for obstacles
        Collider2D obstacle = Physics2D.OverlapCircle(
            new Vector2(position.x, position.y),
            obstacleCheckRadius,
            obstacleLayer
        );

        return obstacle == null;
    }

    /// <summary>
    /// Handle enemy death event
    /// </summary>
    private void OnEnemyDied(GameObject enemy)
    {
        totalKilled++;

        // Don't immediately return to pool - let death animation finish
        // The enemy will be returned to pool after DestroyAfterDeath coroutine
        // OR we can start a delayed return here
        StartCoroutine(DelayedPoolReturn(enemy));
    }

    /// <summary>
    /// Return enemy to pool after death animation completes
    /// </summary>
    private System.Collections.IEnumerator DelayedPoolReturn(GameObject enemy)
    {
        if (enemy == null) yield break;

        // Wait for death animation (should match deathAnimLength in EnemyAnimated)
        yield return new WaitForSeconds(2f);

        if (enemy != null && enemy.activeInHierarchy)
        {
            pool.Return(enemy);
        }
    }

    /// <summary>
    /// Get current alive enemy count
    /// </summary>
    public int GetAliveCount()
    {
        return pool.GetTotalActiveCount();
    }

    /// <summary>
    /// Reset statistics
    /// </summary>
    public void ResetStats()
    {
        totalSpawned = 0;
        totalKilled = 0;
    }

    /// <summary>
    /// Clear all enemies
    /// </summary>
    public void ClearAllEnemies()
    {
        pool.ClearAll();
        ResetStats();
    }

    /// <summary>
    /// Weighted random selection from spawn entries
    /// </summary>
    public EnemySpawnEntry SelectWeightedRandom(EnemySpawnEntry[] entries, float maxAffordableCost)
    {
        if (entries == null || entries.Length == 0)
            return null;

        // Filter affordable entries
        List<EnemySpawnEntry> affordable = new List<EnemySpawnEntry>();
        float totalWeight = 0f;

        foreach (var entry in entries)
        {
            if (entry != null && entry.prefab != null && entry.cost <= maxAffordableCost)
            {
                affordable.Add(entry);
                totalWeight += entry.weight;
            }
        }

        if (affordable.Count == 0)
            return null;

        // Weighted random selection
        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in affordable)
        {
            cumulative += entry.weight;
            if (randomValue <= cumulative)
            {
                return entry;
            }
        }

        return affordable[affordable.Count - 1];
    }
}
