using UnityEngine;

/// <summary>
/// Simple wave configuration - just the essentials
/// </summary>
[CreateAssetMenu(fileName = "Wave", menuName = "Simple Wave System/Wave", order = 1)]
public class SimpleWave : ScriptableObject
{
    [Header("Wave Settings")]
    [Tooltip("How many enemies to spawn this wave")]
    public int enemiesToSpawn = 10;

    [Tooltip("How many enemies per second")]
    public float spawnRate = 1.0f;

    [Tooltip("Maximum enemies alive at once")]
    public int maxAlive = 8;

    [Header("Enemy Types")]
    [Tooltip("List of enemy prefabs to spawn (will randomly pick from this list)")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Area")]
    [Tooltip("Minimum distance from player to spawn")]
    public float spawnMinRadius = 8f;

    [Tooltip("Maximum distance from player to spawn")]
    public float spawnMaxRadius = 12f;

    [Header("Break Time")]
    [Tooltip("Seconds to wait before next wave")]
    public float breakDuration = 5f;
}
