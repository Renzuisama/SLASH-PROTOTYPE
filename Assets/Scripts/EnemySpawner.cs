using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs; // Array of enemy prefab variants

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f; // Spawn every 3 seconds
    [SerializeField] private Transform player; // Reference to player for distance checking

    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-10f, -10f); // Bottom-left corner
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(10f, 10f);   // Top-right corner
    [SerializeField] private float minDistanceFromPlayer = 5f; // Don't spawn too close to player
    [SerializeField] private LayerMask obstacleLayer; // Layer for walls/obstacles to avoid

    [Header("Optional Limits")]
    [SerializeField] private bool limitMaxEnemies = false;
    [SerializeField] private int maxEnemies = 50; // Prevent too many enemies at once

    private int currentEnemyCount = 0;

    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("EnemySpawner: No player found! Assign Player in Inspector or set Player tag.");
                return;
            }
        }

        // Validate enemy prefabs
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("EnemySpawner: No enemy prefabs assigned! Add enemy prefabs in Inspector.");
            return;
        }

        // Start spawning
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check if we've hit the enemy limit
            if (limitMaxEnemies && currentEnemyCount >= maxEnemies)
            {
                Debug.Log($"Max enemies ({maxEnemies}) reached. Waiting for enemies to die...");
                continue;
            }

            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        // Pick a random enemy prefab
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];

        if (enemyPrefab == null)
        {
            Debug.LogWarning($"Enemy prefab at index {randomIndex} is null!");
            return;
        }

        // Find a valid spawn position
        Vector2 spawnPos = GetRandomSpawnPosition();

        // Spawn the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.name = $"{enemyPrefab.name}_{currentEnemyCount}";

        currentEnemyCount++;

        // Listen for enemy death to decrement count
        EnemyAnimated enemyScript = enemy.GetComponent<EnemyAnimated>();
        if (enemyScript != null)
        {
            // We'll add an event system for this in the next step
            StartCoroutine(WaitForEnemyDeath(enemy));
        }

        Debug.Log($"Spawned {enemy.name} at {spawnPos}. Current enemy count: {currentEnemyCount}");
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomPos;
        int attempts = 0;
        int maxAttempts = 50;

        do
        {
            // Generate random position within spawn area
            randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            attempts++;

            // If too many attempts, just use the position anyway
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Could not find valid spawn position after 50 attempts. Spawning anyway.");
                break;
            }

            // Check if position is too close to player
            if (player != null && Vector2.Distance(randomPos, player.position) < minDistanceFromPlayer)
            {
                continue; // Too close to player, try again
            }

            // Check if position overlaps with walls/obstacles
            if (IsPositionBlocked(randomPos))
            {
                continue; // Position is blocked, try again
            }

            // Position is valid!
            break;

        } while (attempts < maxAttempts);

        return randomPos;
    }

    private bool IsPositionBlocked(Vector2 position)
    {
        // Check if there's a collider at this position (wall, obstacle, etc.)
        float checkRadius = 0.5f; // Adjust based on enemy size
        Collider2D hit = Physics2D.OverlapCircle(position, checkRadius, obstacleLayer);

        return hit != null; // Returns true if blocked
    }

    // Simple coroutine to wait for enemy to be destroyed
    private IEnumerator WaitForEnemyDeath(GameObject enemy)
    {
        while (enemy != null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        currentEnemyCount--;
        Debug.Log($"Enemy died. Current enemy count: {currentEnemyCount}");
    }

    // Debug visualization in Scene view
    private void OnDrawGizmosSelected()
    {
        // Draw spawn area
        Gizmos.color = Color.cyan;
        Vector3 bottomLeft = new Vector3(spawnAreaMin.x, spawnAreaMin.y, 0);
        Vector3 bottomRight = new Vector3(spawnAreaMax.x, spawnAreaMin.y, 0);
        Vector3 topLeft = new Vector3(spawnAreaMin.x, spawnAreaMax.y, 0);
        Vector3 topRight = new Vector3(spawnAreaMax.x, spawnAreaMax.y, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Draw minimum distance from player circle
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}
