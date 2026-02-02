using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject healthPickupPrefab;
    [SerializeField] private float spawnInterval = 50f; // Spawn every 50 seconds
    [SerializeField] private int maxHealthPickups = 3; // Maximum number of health pickups on map at once

    [Header("Spawn Area")]
    [SerializeField] private bool useSpawnBounds = true;
    [SerializeField] private float minWorldX = -50f;
    [SerializeField] private float maxWorldX = 50f;
    [SerializeField] private float minWorldY = -50f;
    [SerializeField] private float maxWorldY = 50f;

    [Header("Spawn Validation")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private int maxSpawnAttempts = 20;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private float nextSpawnTime;
    private int currentHealthPickupCount = 0;

    private void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;

        if (healthPickupPrefab == null)
        {
            Debug.LogError("[Health Spawner] Health pickup prefab is not assigned!");
        }
    }

    private void Update()
    {
        // Check if it's time to spawn and we haven't reached the max limit
        if (Time.time >= nextSpawnTime && currentHealthPickupCount < maxHealthPickups)
        {
            SpawnHealthPickup();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnHealthPickup()
    {
        if (healthPickupPrefab == null)
        {
            Debug.LogError("[Health Spawner] Cannot spawn - prefab is null!");
            return;
        }

        Vector2 spawnPosition = Vector2.zero;
        bool validPositionFound = false;

        // Try to find a valid spawn position
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // Generate random position within bounds
            float randomX = Random.Range(minWorldX, maxWorldX);
            float randomY = Random.Range(minWorldY, maxWorldY);
            Vector2 testPos = new Vector2(randomX, randomY);

            // Check if position is clear (no obstacles)
            if (!Physics2D.OverlapCircle(testPos, checkRadius, obstacleLayer))
            {
                spawnPosition = testPos;
                validPositionFound = true;
                break;
            }
        }

        if (validPositionFound)
        {
            GameObject healthPickup = Instantiate(healthPickupPrefab, spawnPosition, Quaternion.identity);
            currentHealthPickupCount++;

            if (debugMode)
            {
                Debug.Log($"[Health Spawner] Spawned health pickup at ({spawnPosition.x:F1}, {spawnPosition.y:F1}) - Total: {currentHealthPickupCount}/{maxHealthPickups}");
            }

            // Subscribe to destruction event to track count
            HealthPickup pickup = healthPickup.GetComponent<HealthPickup>();
            if (pickup != null)
            {
                // We'll use OnDestroy callback through a separate component
                healthPickup.AddComponent<HealthPickupTracker>().Initialize(this);
            }
        }
        else
        {
            if (debugMode)
            {
                Debug.LogWarning($"[Health Spawner] Could not find valid spawn position after {maxSpawnAttempts} attempts");
            }
        }
    }

    public void OnHealthPickupDestroyed()
    {
        currentHealthPickupCount--;
        if (debugMode)
        {
            Debug.Log($"[Health Spawner] Health pickup destroyed - Remaining: {currentHealthPickupCount}/{maxHealthPickups}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (useSpawnBounds)
        {
            // Draw spawn area bounds in Scene view
            Gizmos.color = Color.green;
            Vector3 topLeft = new Vector3(minWorldX, maxWorldY, 0);
            Vector3 topRight = new Vector3(maxWorldX, maxWorldY, 0);
            Vector3 bottomLeft = new Vector3(minWorldX, minWorldY, 0);
            Vector3 bottomRight = new Vector3(maxWorldX, minWorldY, 0);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}

// Helper component to track when health pickups are destroyed
public class HealthPickupTracker : MonoBehaviour
{
    private HealthSpawner spawner;

    public void Initialize(HealthSpawner healthSpawner)
    {
        spawner = healthSpawner;
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnHealthPickupDestroyed();
        }
    }
}
