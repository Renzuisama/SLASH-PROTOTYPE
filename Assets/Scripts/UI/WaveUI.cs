using UnityEngine;
using TMPro;

/// <summary>
/// UI component to display wave information
/// </summary>
public class WaveUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("TextMeshPro component for wave display")]
    public TextMeshProUGUI waveText;

    [Tooltip("TextMeshPro component for intermission countdown (optional)")]
    public TextMeshProUGUI intermissionText;

    [Tooltip("GameObject to show during intermission (optional)")]
    public GameObject intermissionPanel;

    [Header("Format")]
    [Tooltip("Text format for wave display. Use {0} for wave number.")]
    public string waveFormat = "Wave: {0}";

    [Tooltip("Text format for intermission. Use {0} for seconds remaining.")]
    public string intermissionFormat = "Next Wave in: {0}s";

    [Header("Optional - Additional Info")]
    [Tooltip("Show alive enemy count (optional)")]
    public TextMeshProUGUI aliveCountText;

    [Tooltip("Format for alive count. Use {0} for count.")]
    public string aliveCountFormat = "Enemies: {0}";

    [Tooltip("Show kill count (optional)")]
    public TextMeshProUGUI killCountText;

    [Tooltip("Format for kill count. Use {0} for kills.")]
    public string killCountFormat = "Kills: {0}";

    private WaveDirector waveDirector;
    private int currentWave = 0;

    private void Start()
    {
        // Find WaveDirector in scene
        waveDirector = FindObjectOfType<WaveDirector>();

        if (waveDirector == null)
        {
            Debug.LogWarning("WaveUI: No WaveDirector found in scene!");
            enabled = false;
            return;
        }

        // Subscribe to wave events
        waveDirector.OnWaveStarted += HandleWaveStarted;
        waveDirector.OnWaveEnded += HandleWaveEnded;
        waveDirector.OnIntermissionTick += HandleIntermissionTick;

        // Hide intermission UI at start
        if (intermissionPanel != null)
        {
            intermissionPanel.SetActive(false);
        }

        if (intermissionText != null)
        {
            intermissionText.gameObject.SetActive(false);
        }

        // Initial update
        UpdateWaveText(0);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (waveDirector != null)
        {
            waveDirector.OnWaveStarted -= HandleWaveStarted;
            waveDirector.OnWaveEnded -= HandleWaveEnded;
            waveDirector.OnIntermissionTick -= HandleIntermissionTick;
        }
    }

    private void Update()
    {
        if (waveDirector == null) return;

        // Update alive count
        if (aliveCountText != null)
        {
            aliveCountText.text = string.Format(aliveCountFormat, waveDirector.AliveCount);
        }

        // Update kill count
        if (killCountText != null)
        {
            // Show wave kills / kill target for KF mode, or total kills for VS mode
            if (waveDirector.GetCurrentWave() != null &&
                waveDirector.GetCurrentWave().mode == WaveConfig.WaveMode.KillingFloor)
            {
                killCountText.text = $"Kills: {waveDirector.WaveKillCount}/{waveDirector.WaveKillTarget}";
            }
            else
            {
                killCountText.text = string.Format(killCountFormat, waveDirector.TotalKilled);
            }
        }
    }

    /// <summary>
    /// Called when a wave starts
    /// </summary>
    private void HandleWaveStarted(int waveIndex)
    {
        currentWave = waveIndex;
        UpdateWaveText(waveIndex);

        // Hide intermission UI
        if (intermissionPanel != null)
        {
            intermissionPanel.SetActive(false);
        }

        if (intermissionText != null)
        {
            intermissionText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when a wave ends
    /// </summary>
    private void HandleWaveEnded(int waveIndex)
    {
        // Optional: Add visual feedback when wave ends
        Debug.Log($"WaveUI: Wave {waveIndex} completed!");
    }

    /// <summary>
    /// Called during intermission countdown
    /// </summary>
    private void HandleIntermissionTick(float timeRemaining)
    {
        // Show intermission UI
        if (intermissionPanel != null && !intermissionPanel.activeSelf)
        {
            intermissionPanel.SetActive(true);
        }

        if (intermissionText != null)
        {
            intermissionText.gameObject.SetActive(true);
            intermissionText.text = string.Format(intermissionFormat, Mathf.CeilToInt(timeRemaining));
        }
    }

    /// <summary>
    /// Update wave text display
    /// </summary>
    private void UpdateWaveText(int waveIndex)
    {
        if (waveText != null)
        {
            // Display as 1-indexed for players (Wave 1 instead of Wave 0)
            waveText.text = string.Format(waveFormat, waveIndex + 1);
        }
    }

    /// <summary>
    /// Manually set wave text (for testing)
    /// </summary>
    public void SetWaveText(string text)
    {
        if (waveText != null)
        {
            waveText.text = text;
        }
    }
}
