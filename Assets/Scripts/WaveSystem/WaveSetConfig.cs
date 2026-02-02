using UnityEngine;

/// <summary>
/// Container for a sequence of waves
/// </summary>
[CreateAssetMenu(fileName = "WaveSetConfig", menuName = "Wave System/Wave Set Config")]
public class WaveSetConfig : ScriptableObject
{
    [Header("Wave Sequence")]
    [Tooltip("Ordered list of wave configurations")]
    public WaveConfig[] waves;

    [Header("Endless Mode")]
    [Tooltip("After last wave, loop back with scaling")]
    public bool endlessMode = false;

    [Tooltip("Difficulty multiplier applied each loop in endless mode")]
    [Min(1f)]
    public float endlessScaling = 1.2f;

    [Tooltip("Max difficulty multiplier cap in endless mode")]
    [Min(1f)]
    public float maxDifficultyMultiplier = 5f;

    /// <summary>
    /// Get wave at index, handling endless looping
    /// </summary>
    public WaveConfig GetWave(int index, out float difficultyMultiplier)
    {
        difficultyMultiplier = 1f;

        if (waves == null || waves.Length == 0)
            return null;

        if (index < waves.Length)
        {
            return waves[index];
        }

        if (!endlessMode)
            return null;

        // Endless mode: loop with scaling
        int loopCount = index / waves.Length;
        int loopIndex = index % waves.Length;

        difficultyMultiplier = Mathf.Min(
            Mathf.Pow(endlessScaling, loopCount),
            maxDifficultyMultiplier
        );

        return waves[loopIndex];
    }

    /// <summary>
    /// Check if there are more waves
    /// </summary>
    public bool HasWave(int index)
    {
        if (waves == null || waves.Length == 0)
            return false;

        return index < waves.Length || endlessMode;
    }
}
