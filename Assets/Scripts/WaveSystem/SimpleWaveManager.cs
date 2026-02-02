using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages wave progression - super simple version
/// </summary>
public class SimpleWaveManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The wave set to use")]
    public SimpleWaveSet waveSet;

    [Header("Player Reference")]
    [Tooltip("The player transform (for spawn position calculation)")]
    public Transform player;

    [Header("Spawn Settings")]
    [Tooltip("LayerMask for obstacles (walls, etc)")]
    public LayerMask obstacleLayer;

    [Tooltip("How many times to try finding a valid spawn position")]
    public int maxSpawnAttempts = 20;

    [Tooltip("Enable spawn boundary checking (disable to match original behavior)")]
    public bool useSpawnBounds = false;

    [Tooltip("Minimum world X position for spawning")]
    public float minWorldX = -50f;

    [Tooltip("Maximum world X position for spawning")]
    public float maxWorldX = 50f;

    [Tooltip("Minimum world Y position for spawning")]
    public float minWorldY = -50f;

    [Tooltip("Maximum world Y position for spawning")]
    public float maxWorldY = 50f;

    [Header("Audio")]
    [Tooltip("Reference to the MusicManager (optional - will auto-find if not set)")]
    public MusicManager musicManager;

    [Tooltip("Track index to play on final wave (-1 = no change)")]
    public int finalWaveBGMTrack = -1;

    [Header("Difficulty Scaling")]
    [Tooltip("Enable difficulty scaling per wave")]
    public bool enableDifficultyScaling = true;

    [Tooltip("HP multiplier per wave (e.g., 1.1 = 10% more HP each wave)")]
    [Range(1f, 2f)]
    public float hpScalingPerWave = 1.15f;

    [Tooltip("Damage multiplier per wave (e.g., 1.1 = 10% more damage each wave)")]
    [Range(1f, 2f)]
    public float damageScalingPerWave = 1.1f;

    [Tooltip("Speed multiplier per wave (e.g., 1.05 = 5% faster each wave)")]
    [Range(1f, 2f)]
    public float speedScalingPerWave = 1.05f;

    [Header("Debug")]
    [Tooltip("Show debug messages")]
    public bool debugMode = false;

    // Events for UI
    public System.Action<int, int> OnWaveChanged; // (currentWave, totalWaves)
    public System.Action<int> OnEnemyCountChanged; // (aliveCount)
    public System.Action<int, int> OnKillsChanged; // (currentKills, targetKills)
    public System.Action<float> OnBreakStarted; // (breakDuration)

    // Current state
    private int currentWaveIndex = 0;
    private int loopCount = 0;
    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private int targetKills = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool isRunning = false;

    // Public properties for UI
    public int CurrentWave => currentWaveIndex + 1;
    public int TotalWaves => waveSet != null ? waveSet.waves.Length : 0;
    public int AliveCount => aliveEnemies.Count;
    public int KillCount => enemiesKilled;
    public int KillTarget => targetKills;

    // Public properties for difficulty scaling
    public float CurrentHPMultiplier => enableDifficultyScaling ? Mathf.Pow(hpScalingPerWave, currentWaveIndex) : 1f;
    public float CurrentDamageMultiplier => enableDifficultyScaling ? Mathf.Pow(damageScalingPerWave, currentWaveIndex) : 1f;
    public float CurrentSpeedMultiplier => enableDifficultyScaling ? Mathf.Pow(speedScalingPerWave, currentWaveIndex) : 1f;

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
                Debug.LogError("SimpleWaveManager: No player found! Assign player or tag player with 'Player'");
                enabled = false;
                return;
            }
        }

        // Find MusicManager if not assigned
        if (musicManager == null)
        {
            musicManager = FindFirstObjectByType<MusicManager>();
            if (musicManager == null && finalWaveBGMTrack >= 0)
            {
                Debug.LogWarning("SimpleWaveManager: Final wave BGM track set but no MusicManager found in scene!");
            }
        }

        // Validate wave set
        if (waveSet == null || waveSet.waves.Length == 0)
        {
            Debug.LogError("SimpleWaveManager: No wave set assigned or wave set is empty!");
            enabled = false;
            return;
        }

        // Subscribe to enemy death events
        EnemyDeathBroadcaster.OnEnemyDied += HandleEnemyDeath;

        // Start first wave
        StartCoroutine(RunWaves());
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        EnemyDeathBroadcaster.OnEnemyDied -= HandleEnemyDeath;
    }

    /// <summary>
    /// Main wave loop
    /// </summary>
    private IEnumerator RunWaves()
    {
        isRunning = true;

        while (isRunning)
        {
            // Get current wave (with endless mode support)
            SimpleWave wave = GetCurrentWave();
            if (wave == null)
            {
                Debug.LogError("SimpleWaveManager: Current wave is null!");
                yield break;
            }

            // Calculate target kills (with endless multiplier)
            float multiplier = loopCount > 0 ? Mathf.Pow(waveSet.endlessMultiplier, loopCount) : 1f;
            targetKills = Mathf.RoundToInt(wave.enemiesToSpawn * multiplier);

            if (debugMode)
            {
                Debug.Log($"[Wave {currentWaveIndex + 1}] Starting - Spawn {targetKills} enemies, Loop {loopCount}");
            }

            // Check if this is the second-to-last wave and switch BGM if needed
            // Play final BGM when the LAST wave starts
            if (!waveSet.endlessMode && currentWaveIndex == waveSet.waves.Length - 1)
            {
                if (musicManager != null)
                    musicManager.PlayFinalLevelBgm(withFade: true);
            }


            // Reset counters
            enemiesSpawned = 0;
            enemiesKilled = 0;

            // Notify UI
            OnWaveChanged?.Invoke(currentWaveIndex + 1, TotalWaves);
            OnKillsChanged?.Invoke(0, targetKills);

            // Spawn enemies until target is reached
            yield return StartCoroutine(SpawnWave(wave));

            // Wait for all enemies to be killed
            while (enemiesKilled < targetKills)
            {
                OnKillsChanged?.Invoke(enemiesKilled, targetKills);
                yield return null;
            }

            if (debugMode)
            {
                Debug.Log($"[Wave {currentWaveIndex + 1}] Complete - {enemiesKilled}/{targetKills} killed");
            }

            // Move to next wave
            currentWaveIndex++;

            // Check if we've completed all waves
            if (currentWaveIndex >= waveSet.waves.Length)
            {
                if (waveSet.endlessMode)
                {
                    // Loop back to first wave with increased difficulty
                    currentWaveIndex = 0;
                    loopCount++;
                    if (debugMode)
                    {
                        Debug.Log($"[Endless Mode] Starting loop {loopCount + 1} with {waveSet.endlessMultiplier}x multiplier");
                    }
                }
                else
                {
                    // All waves complete!
                    if (debugMode)
                    {
                        Debug.Log("[Wave Manager] All waves completed!");
                    }
                    isRunning = false;
                    yield break;
                }
            }

            // Break between waves
            if (wave.breakDuration > 0)
            {
                if (debugMode)
                {
                    Debug.Log($"[Wave Manager] Break for {wave.breakDuration} seconds");
                }
                OnBreakStarted?.Invoke(wave.breakDuration);
                yield return new WaitForSeconds(wave.breakDuration);
            }
        }
    }

    /// <summary>
    /// Spawn all enemies for current wave
    /// </summary>
    private IEnumerator SpawnWave(SimpleWave wave)
    {
        float spawnInterval = 1f / wave.spawnRate;
        float timeSinceLastSpawn = 0f;

        while (enemiesSpawned < targetKills)
        {
            timeSinceLastSpawn += Time.deltaTime;

            // Check if we should spawn and haven't hit max alive
            if (timeSinceLastSpawn >= spawnInterval && aliveEnemies.Count < wave.maxAlive)
            {
                SpawnEnemy(wave);
                timeSinceLastSpawn = 0f;
            }

            yield return null;
        }

        if (debugMode)
        {
            Debug.Log($"[Wave Manager] Finished spawning {enemiesSpawned} enemies");
        }
    }

    /// <summary>
    /// Spawn a single enemy
    /// </summary>
    private void SpawnEnemy(SimpleWave wave)
    {
        // Validate enemy prefabs
        if (wave.enemyPrefabs == null || wave.enemyPrefabs.Length == 0)
        {
            Debug.LogError("SimpleWaveManager: No enemy prefabs assigned to wave!");
            return;
        }

        // Pick random enemy type from array
        GameObject enemyPrefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Length)];

        if (enemyPrefab == null)
        {
            Debug.LogError("SimpleWaveManager: Enemy prefab in array is null!");
            return;
        }

        if (player == null)
        {
            Debug.LogError("SimpleWaveManager: Player is null!");
            return;
        }

        // Try to find valid spawn position
        Vector3 spawnPos = Vector3.zero;
        bool foundValidPosition = false;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // Random position in ring around player
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(wave.spawnMinRadius, wave.spawnMaxRadius);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0f);
            Vector3 testPos = player.position + offset;

            // Check if position is within world bounds
            if (useSpawnBounds)
            {
                if (testPos.x < minWorldX || testPos.x > maxWorldX ||
                    testPos.y < minWorldY || testPos.y > maxWorldY)
                {
                    continue; // Skip this position, it's out of bounds
                }
            }

            // Check if position is valid (not in wall)
            Collider2D hit = Physics2D.OverlapCircle(testPos, 0.5f, obstacleLayer);
            if (hit == null)
            {
                spawnPos = testPos;
                foundValidPosition = true;
                break;
            }
        }

        if (!foundValidPosition)
        {
            if (debugMode)
            {
                Debug.LogWarning($"[Wave Manager] Could not find valid spawn position after {maxSpawnAttempts} attempts");
            }
            return;
        }

        // Spawn enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        aliveEnemies.Add(enemy);
        enemiesSpawned++;

        // Apply difficulty scaling if enabled
        if (enableDifficultyScaling)
        {
            ApplyDifficultyScaling(enemy);
        }

        OnEnemyCountChanged?.Invoke(aliveEnemies.Count);

        if (debugMode)
        {
            Debug.Log($"[Wave Manager] Spawned {enemy.name} at {spawnPos} ({enemiesSpawned}/{targetKills})");
        }
    }

    /// <summary>
    /// Apply difficulty scaling to spawned enemy
    /// </summary>
    private void ApplyDifficultyScaling(GameObject enemy)
    {
        // Try to get EnemyAnimated component
        EnemyAnimated enemyScript = enemy.GetComponent<EnemyAnimated>();
        if (enemyScript != null)
        {
            enemyScript.ApplyDifficultyScaling(CurrentHPMultiplier, CurrentDamageMultiplier, CurrentSpeedMultiplier);

            if (debugMode)
            {
                Debug.Log($"[Wave Manager] Applied scaling to {enemy.name}: HP x{CurrentHPMultiplier:F2}, DMG x{CurrentDamageMultiplier:F2}, SPD x{CurrentSpeedMultiplier:F2}");
            }
        }
    }

    /// <summary>
    /// Handle enemy death event
    /// </summary>
    private void HandleEnemyDeath(GameObject enemy)
    {
        if (aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
            enemiesKilled++;

            OnEnemyCountChanged?.Invoke(aliveEnemies.Count);
            OnKillsChanged?.Invoke(enemiesKilled, targetKills);

            if (debugMode)
            {
                Debug.Log($"[Wave Manager] Enemy killed ({enemiesKilled}/{targetKills})");
            }
        }
    }

    /// <summary>
    /// Get current wave with endless mode support
    /// </summary>
    private SimpleWave GetCurrentWave()
    {
        if (waveSet == null || waveSet.waves.Length == 0)
        {
            return null;
        }

        int index = currentWaveIndex % waveSet.waves.Length;
        return waveSet.waves[index];
    }
}
