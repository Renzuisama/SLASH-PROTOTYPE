using UnityEngine;

/// <summary>
/// Container for multiple waves
/// </summary>
[CreateAssetMenu(fileName = "WaveSet", menuName = "Simple Wave System/Wave Set", order = 2)]
public class SimpleWaveSet : ScriptableObject
{
    [Header("Wave List")]
    [Tooltip("List of waves in order")]
    public SimpleWave[] waves;

    [Header("Endless Mode")]
    [Tooltip("Loop back to wave 1 after completing all waves")]
    public bool endlessMode = false;

    [Tooltip("Multiply enemy count by this amount each loop (e.g., 1.2 = 20% more)")]
    public float endlessMultiplier = 1.2f;
}
