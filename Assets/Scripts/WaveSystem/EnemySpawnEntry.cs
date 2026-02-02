using UnityEngine;

/// <summary>
/// ScriptableObject defining a spawnable enemy type with cost and availability
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnEntry", menuName = "Wave System/Enemy Spawn Entry")]
public class EnemySpawnEntry : ScriptableObject
{
    [Header("Enemy Configuration")]
    [Tooltip("The enemy prefab to spawn")]
    public GameObject prefab;

    [Tooltip("Spawn budget cost (used in VS mode)")]
    [Min(1)]
    public int cost = 1;

    [Tooltip("Weighted random selection chance (higher = more common)")]
    [Min(0.1f)]
    public float weight = 1f;

    [Header("Availability")]
    [Tooltip("Minimum game time (seconds) when this enemy becomes available. 0 = always available.")]
    [Min(0)]
    public float minTimeSeconds = 0f;

    [Tooltip("Maximum game time (seconds) when this enemy is available. 0 = no limit.")]
    [Min(0)]
    public float maxTimeSeconds = 0f;

    [Tooltip("Minimum wave index when available (0-based). -1 = no restriction.")]
    public int minWaveIndex = -1;

    [Tooltip("Maximum wave index when available (0-based). -1 = no restriction.")]
    public int maxWaveIndex = -1;

    /// <summary>
    /// Check if this entry is available at the given time and wave
    /// </summary>
    public bool IsAvailable(float gameTime, int waveIndex)
    {
        // Check time constraints
        if (minTimeSeconds > 0 && gameTime < minTimeSeconds)
            return false;

        if (maxTimeSeconds > 0 && gameTime > maxTimeSeconds)
            return false;

        // Check wave constraints
        if (minWaveIndex >= 0 && waveIndex < minWaveIndex)
            return false;

        if (maxWaveIndex >= 0 && waveIndex > maxWaveIndex)
            return false;

        return true;
    }
}
