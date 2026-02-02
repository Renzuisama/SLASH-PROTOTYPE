using UnityEngine;

/// <summary>
/// Configuration for a single wave (supports both VS continuous and KF discrete modes)
/// </summary>
[CreateAssetMenu(fileName = "WaveConfig", menuName = "Wave System/Wave Config")]
public class WaveConfig : ScriptableObject
{
    public enum WaveMode
    {
        VampireSurvivors, // Continuous spawning with budget
        KillingFloor      // Discrete spawning with kill target
    }

    [Header("Wave Type")]
    public WaveMode mode = WaveMode.VampireSurvivors;

    [Header("Duration (VS Mode)")]
    [Tooltip("How long this wave lasts (seconds). Used in VS mode.")]
    [Min(1)]
    public float durationSeconds = 60f;

    [Header("Spawn Budget (VS Mode)")]
    [Tooltip("Base points gained per second for spawning")]
    [Min(0.1f)]
    public float pointsPerSecond = 2f;

    [Tooltip("Optional curve to modify points per second over wave duration (0-1 normalized time)")]
    public AnimationCurve pointsPerSecondCurve = AnimationCurve.Linear(0, 1, 1, 1);

    [Header("Discrete Spawning (KF Mode)")]
    [Tooltip("Total enemies to spawn in this wave (KF mode)")]
    [Min(1)]
    public int totalToSpawn = 20;

    [Tooltip("Kill target to complete wave (KF mode). If 0, uses totalToSpawn.")]
    [Min(0)]
    public int killTarget = 0;

    [Tooltip("Spawns per second in KF mode")]
    [Min(0.1f)]
    public float spawnsPerSecond = 1f;

    [Header("General Settings")]
    [Tooltip("Maximum enemies alive at once")]
    [Min(1)]
    public int maxAlive = 50;

    [Header("Spawn Area")]
    [Tooltip("Minimum spawn radius around player (ring spawn)")]
    [Min(1f)]
    public float spawnRadiusMin = 15f;

    [Tooltip("Maximum spawn radius around player (ring spawn)")]
    [Min(1f)]
    public float spawnRadiusMax = 25f;

    [Tooltip("World bounds minimum (optional clamp)")]
    public Vector2 worldBoundsMin = new Vector2(-50, -50);

    [Tooltip("World bounds maximum (optional clamp)")]
    public Vector2 worldBoundsMax = new Vector2(50, 50);

    [Tooltip("Use world bounds clamping")]
    public bool useWorldBounds = true;

    [Header("Enemy Types")]
    [Tooltip("Available enemy spawn entries for this wave")]
    public EnemySpawnEntry[] entries;

    [Header("Boss/Elite")]
    [Tooltip("Optional boss to spawn")]
    public GameObject bossPrefab;

    [Tooltip("Spawn boss at this time (seconds into wave). 0 = disabled.")]
    [Min(0)]
    public float bossTimeSeconds = 0f;

    [Tooltip("Spawn boss at wave start")]
    public bool spawnBossAtStart = false;

    [Header("Difficulty")]
    [Tooltip("Difficulty multiplier applied to spawn rates/budgets")]
    [Min(0.1f)]
    public float difficultyMultiplier = 1f;

    [Header("Intermission (KF Mode)")]
    [Tooltip("Intermission duration after this wave completes (KF mode only)")]
    [Min(0)]
    public float intermissionDuration = 10f;

    /// <summary>
    /// Get effective kill target for KF mode
    /// </summary>
    public int GetKillTarget()
    {
        return killTarget > 0 ? killTarget : totalToSpawn;
    }

    /// <summary>
    /// Get points per second at a given normalized time (0-1) in the wave
    /// </summary>
    public float GetPointsPerSecond(float normalizedTime)
    {
        float curveMultiplier = pointsPerSecondCurve.Evaluate(normalizedTime);
        return pointsPerSecond * curveMultiplier * difficultyMultiplier;
    }

    /// <summary>
    /// Get available entries at the given game time and wave index
    /// </summary>
    public EnemySpawnEntry[] GetAvailableEntries(float gameTime, int waveIndex)
    {
        if (entries == null || entries.Length == 0)
            return new EnemySpawnEntry[0];

        System.Collections.Generic.List<EnemySpawnEntry> available = new System.Collections.Generic.List<EnemySpawnEntry>();
        foreach (var entry in entries)
        {
            if (entry != null && entry.prefab != null && entry.IsAvailable(gameTime, waveIndex))
            {
                available.Add(entry);
            }
        }
        return available.ToArray();
    }
}
