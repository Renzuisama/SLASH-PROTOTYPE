using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Main wave director controlling wave progression and spawn budget
/// </summary>
[RequireComponent(typeof(SpawnService))]
public class WaveDirector : MonoBehaviour
{
    public enum WaveState
    {
        Warmup,
        WaveActive,
        Intermission,
        Completed
    }

    [Header("Configuration")]
    [Tooltip("Wave set configuration ScriptableObject")]
    public WaveSetConfig waveSetConfig;

    [Tooltip("Warmup duration before first wave starts")]
    [Min(0)]
    public float warmupDuration = 3f;

    [Header("Debug")]
    [Tooltip("Enable debug logging")]
    public bool debugMode = false;

    // State
    private WaveState currentState = WaveState.Warmup;
    private int currentWaveIndex = 0;
    private float currentWaveDuration = 0f;
    private float gameTime = 0f;
    private float waveStartTime = 0f;
    private float spawnBudget = 0f;
    private float endlessMultiplier = 1f;

    // KF mode tracking
    private int waveSpawnedCount = 0;
    private int waveKilledCount = 0;
    private int lastKilledCount = 0;

    // Boss tracking
    private bool bossSpawned = false;

    private SpawnService spawnService;
    private WaveConfig currentWave;
    private Coroutine waveCoroutine;

    // Events
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveEnded;
    public event Action<GameObject> OnEnemySpawned;
    public event Action OnEnemyKilled;
    public event Action<float> OnIntermissionTick;

    // Public properties
    public WaveState CurrentState => currentState;
    public int CurrentWaveIndex => currentWaveIndex;
    public float GameTime => gameTime;
    public float SpawnBudget => spawnBudget;
    public int AliveCount => spawnService != null ? spawnService.GetAliveCount() : 0;
    public int TotalSpawned => spawnService != null ? spawnService.TotalSpawned : 0;
    public int TotalKilled => spawnService != null ? spawnService.TotalKilled : 0;
    public int WaveKillTarget => currentWave != null ? Mathf.RoundToInt(currentWave.GetKillTarget() * endlessMultiplier) : 0;
    public int WaveKillCount => waveKilledCount;
    public int WaveSpawnedCount => waveSpawnedCount;

    private void Awake()
    {
        spawnService = GetComponent<SpawnService>();
    }

    private void Start()
    {
        if (waveSetConfig == null)
        {
            Debug.LogError("WaveDirector: No WaveSetConfig assigned!");
            enabled = false;
            return;
        }

        StartWaveSystem();
    }

    private void OnEnable()
    {
        EnemyDeathBroadcaster.OnEnemyDied += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyDeathBroadcaster.OnEnemyDied -= HandleEnemyDeath;

        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
    }

    /// <summary>
    /// Start the wave system
    /// </summary>
    public void StartWaveSystem()
    {
        currentState = WaveState.Warmup;
        currentWaveIndex = 0;
        gameTime = 0f;
        endlessMultiplier = 1f;

        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }

        waveCoroutine = StartCoroutine(WaveSystemCoroutine());
    }

    /// <summary>
    /// Main wave system state machine
    /// </summary>
    private IEnumerator WaveSystemCoroutine()
    {
        // Warmup
        if (warmupDuration > 0)
        {
            currentState = WaveState.Warmup;
            Log($"Warmup started: {warmupDuration}s");
            yield return new WaitForSeconds(warmupDuration);
        }

        // Wave loop
        while (waveSetConfig.HasWave(currentWaveIndex))
        {
            currentWave = waveSetConfig.GetWave(currentWaveIndex, out endlessMultiplier);

            if (currentWave == null)
            {
                Debug.LogError($"WaveDirector: Wave {currentWaveIndex} is null!");
                break;
            }

            // Start wave
            yield return StartCoroutine(RunWave());

            // Check if we should continue
            if (!waveSetConfig.HasWave(currentWaveIndex + 1))
            {
                currentState = WaveState.Completed;
                Log("All waves completed!");
                break;
            }

            currentWaveIndex++;
        }
    }

    /// <summary>
    /// Run a single wave
    /// </summary>
    private IEnumerator RunWave()
    {
        currentState = WaveState.WaveActive;
        waveStartTime = gameTime;
        currentWaveDuration = 0f;
        spawnBudget = 0f;
        waveSpawnedCount = 0;
        waveKilledCount = 0;
        lastKilledCount = spawnService.TotalKilled;
        bossSpawned = false;

        Log($"Wave {currentWaveIndex} started - Mode: {currentWave.mode}, Multiplier: {endlessMultiplier:F2}");
        OnWaveStarted?.Invoke(currentWaveIndex);

        // Spawn boss at start if configured
        if (currentWave.spawnBossAtStart && currentWave.bossPrefab != null)
        {
            SpawnBoss();
        }

        // Run wave based on mode
        if (currentWave.mode == WaveConfig.WaveMode.VampireSurvivors)
        {
            yield return StartCoroutine(RunVampireSurvivorsWave());
        }
        else
        {
            yield return StartCoroutine(RunKillingFloorWave());
        }

        Log($"Wave {currentWaveIndex} ended");
        OnWaveEnded?.Invoke(currentWaveIndex);

        // Intermission (KF mode only)
        if (currentWave.mode == WaveConfig.WaveMode.KillingFloor && currentWave.intermissionDuration > 0)
        {
            yield return StartCoroutine(RunIntermission());
        }
    }

    /// <summary>
    /// Run Vampire Survivors style wave (continuous spawning with budget)
    /// </summary>
    private IEnumerator RunVampireSurvivorsWave()
    {
        float waveDuration = currentWave.durationSeconds;
        float bossTime = currentWave.bossTimeSeconds;

        while (currentWaveDuration < waveDuration)
        {
            float deltaTime = Time.deltaTime;
            currentWaveDuration += deltaTime;

            // Accumulate spawn budget
            float normalizedTime = Mathf.Clamp01(currentWaveDuration / waveDuration);
            float pointsPerSec = currentWave.GetPointsPerSecond(normalizedTime) * endlessMultiplier;
            spawnBudget += pointsPerSec * deltaTime;

            // Spawn boss at specific time
            if (!bossSpawned && bossTime > 0 && currentWaveDuration >= bossTime && currentWave.bossPrefab != null)
            {
                SpawnBoss();
            }

            // Spawn enemies from budget
            SpawnFromBudget();

            yield return null;
        }
    }

    /// <summary>
    /// Run Killing Floor style wave (discrete spawning with kill target)
    /// </summary>
    private IEnumerator RunKillingFloorWave()
    {
        int totalToSpawn = Mathf.RoundToInt(currentWave.totalToSpawn * endlessMultiplier);
        int killTarget = Mathf.RoundToInt(currentWave.GetKillTarget() * endlessMultiplier);
        float spawnInterval = 1f / (currentWave.spawnsPerSecond * endlessMultiplier);

        Log($"KF Wave: Spawn {totalToSpawn}, Kill target {killTarget}");

        // Spawn boss at start if configured
        if (currentWave.spawnBossAtStart && currentWave.bossPrefab != null && !bossSpawned)
        {
            SpawnBoss();
        }

        float timeSinceLastSpawn = 0f;

        while (waveKilledCount < killTarget)
        {
            timeSinceLastSpawn += Time.deltaTime;
            currentWaveDuration += Time.deltaTime;

            // Update kill count
            waveKilledCount = spawnService.TotalKilled - lastKilledCount;

            // Spawn enemies if we haven't reached total and it's time
            if (waveSpawnedCount < totalToSpawn && timeSinceLastSpawn >= spawnInterval)
            {
                if (spawnService.GetAliveCount() < currentWave.maxAlive)
                {
                    SpawnSingleEnemy();
                    waveSpawnedCount++;
                    timeSinceLastSpawn = 0f;
                }
            }

            yield return null;
        }

        Log($"KF Wave completed: {waveKilledCount}/{killTarget} killed");
    }

    /// <summary>
    /// Run intermission between waves
    /// </summary>
    private IEnumerator RunIntermission()
    {
        currentState = WaveState.Intermission;
        float intermissionTime = currentWave.intermissionDuration;

        Log($"Intermission started: {intermissionTime}s");

        while (intermissionTime > 0)
        {
            OnIntermissionTick?.Invoke(intermissionTime);
            yield return new WaitForSeconds(1f);
            intermissionTime -= 1f;
        }

        Log("Intermission ended");
    }

    /// <summary>
    /// Spawn enemies using accumulated budget (VS mode)
    /// </summary>
    private void SpawnFromBudget()
    {
        var availableEntries = currentWave.GetAvailableEntries(gameTime, currentWaveIndex);

        if (availableEntries.Length == 0)
            return;

        // Find minimum cost
        int minCost = int.MaxValue;
        foreach (var entry in availableEntries)
        {
            if (entry.cost < minCost)
                minCost = entry.cost;
        }

        // Spawn while we have budget and room
        while (spawnBudget >= minCost && spawnService.GetAliveCount() < currentWave.maxAlive)
        {
            EnemySpawnEntry selected = spawnService.SelectWeightedRandom(availableEntries, spawnBudget);

            if (selected == null)
                break;

            GameObject enemy = spawnService.SpawnEnemy(
                selected.prefab,
                currentWave.spawnRadiusMin,
                currentWave.spawnRadiusMax,
                currentWave.worldBoundsMin,
                currentWave.worldBoundsMax,
                currentWave.useWorldBounds
            );

            if (enemy != null)
            {
                spawnBudget -= selected.cost;
                OnEnemySpawned?.Invoke(enemy);
                Log($"Spawned {selected.prefab.name}, cost {selected.cost}, budget remaining {spawnBudget:F1}");
            }
            else
            {
                break; // Failed to spawn, stop trying
            }
        }
    }

    /// <summary>
    /// Spawn a single enemy (KF mode)
    /// </summary>
    private void SpawnSingleEnemy()
    {
        var availableEntries = currentWave.GetAvailableEntries(gameTime, currentWaveIndex);

        if (availableEntries.Length == 0)
            return;

        EnemySpawnEntry selected = spawnService.SelectWeightedRandom(availableEntries, float.MaxValue);

        if (selected == null)
            return;

        GameObject enemy = spawnService.SpawnEnemy(
            selected.prefab,
            currentWave.spawnRadiusMin,
            currentWave.spawnRadiusMax,
            currentWave.worldBoundsMin,
            currentWave.worldBoundsMax,
            currentWave.useWorldBounds
        );

        if (enemy != null)
        {
            OnEnemySpawned?.Invoke(enemy);
            Log($"Spawned {selected.prefab.name}");
        }
    }

    /// <summary>
    /// Spawn boss enemy
    /// </summary>
    private void SpawnBoss()
    {
        if (bossSpawned || currentWave.bossPrefab == null)
            return;

        GameObject boss = spawnService.SpawnEnemy(
            currentWave.bossPrefab,
            currentWave.spawnRadiusMin,
            currentWave.spawnRadiusMax,
            currentWave.worldBoundsMin,
            currentWave.worldBoundsMax,
            currentWave.useWorldBounds
        );

        if (boss != null)
        {
            bossSpawned = true;
            OnEnemySpawned?.Invoke(boss);
            Log($"Boss spawned: {currentWave.bossPrefab.name}");
        }
    }

    /// <summary>
    /// Handle enemy death event
    /// </summary>
    private void HandleEnemyDeath(GameObject enemy)
    {
        OnEnemyKilled?.Invoke();
    }

    /// <summary>
    /// Debug logging
    /// </summary>
    private void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log($"[WaveDirector] {message}");
        }
    }

    /// <summary>
    /// Get current wave config
    /// </summary>
    public WaveConfig GetCurrentWave()
    {
        return currentWave;
    }

    /// <summary>
    /// Force start next wave (debug/testing)
    /// </summary>
    public void ForceNextWave()
    {
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }

        currentWaveIndex++;
        waveCoroutine = StartCoroutine(WaveSystemCoroutine());
    }
}
