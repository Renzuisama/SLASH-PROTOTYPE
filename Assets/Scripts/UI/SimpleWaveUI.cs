using UnityEngine;
using TMPro;

/// <summary>
/// Simple UI display for wave information
/// </summary>
public class SimpleWaveUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text to show wave number (e.g., 'WAVE: 1/10')")]
    public TextMeshProUGUI waveText;

    [Tooltip("Text to show alive enemy count (e.g., 'ENEMIES: 5')")]
    public TextMeshProUGUI aliveText;

    [Tooltip("Text to show kill progress (e.g., 'KILLS: 7/15')")]
    public TextMeshProUGUI killsText;

    [Tooltip("Text to show break countdown (optional)")]
    public TextMeshProUGUI breakText;

    [Header("Formats")]
    public string waveFormat = "WAVE: {0}/{1}";
    public string aliveFormat = "ENEMIES: {0}";
    public string killsFormat = "KILLS: {0}/{1}";
    public string breakFormat = "Next wave in: {0}s";

    private SimpleWaveManager waveManager;
    private float breakTimeRemaining = 0f;
    private bool isOnBreak = false;

    private void Start()
    {
        // Find wave manager
        waveManager = FindObjectOfType<SimpleWaveManager>();

        if (waveManager == null)
        {
            Debug.LogWarning("SimpleWaveUI: No SimpleWaveManager found!");
            enabled = false;
            return;
        }

        // Subscribe to events
        waveManager.OnWaveChanged += UpdateWaveText;
        waveManager.OnEnemyCountChanged += UpdateAliveText;
        waveManager.OnKillsChanged += UpdateKillsText;
        waveManager.OnBreakStarted += StartBreakCountdown;

        // Hide break text initially
        if (breakText != null)
        {
            breakText.gameObject.SetActive(false);
        }

        // Initial update
        UpdateWaveText(waveManager.CurrentWave, waveManager.TotalWaves);
        UpdateAliveText(waveManager.AliveCount);
        UpdateKillsText(waveManager.KillCount, waveManager.KillTarget);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (waveManager != null)
        {
            waveManager.OnWaveChanged -= UpdateWaveText;
            waveManager.OnEnemyCountChanged -= UpdateAliveText;
            waveManager.OnKillsChanged -= UpdateKillsText;
            waveManager.OnBreakStarted -= StartBreakCountdown;
        }
    }

    private void Update()
    {
        // Update break countdown
        if (isOnBreak && breakText != null)
        {
            breakTimeRemaining -= Time.deltaTime;
            if (breakTimeRemaining <= 0f)
            {
                isOnBreak = false;
                breakText.gameObject.SetActive(false);
            }
            else
            {
                breakText.text = string.Format(breakFormat, Mathf.CeilToInt(breakTimeRemaining));
            }
        }
    }

    /// <summary>
    /// Update wave number display
    /// </summary>
    private void UpdateWaveText(int currentWave, int totalWaves)
    {
        if (waveText != null)
        {
            waveText.text = string.Format(waveFormat, currentWave, totalWaves);
        }
    }

    /// <summary>
    /// Update alive enemy count
    /// </summary>
    private void UpdateAliveText(int aliveCount)
    {
        if (aliveText != null)
        {
            aliveText.text = string.Format(aliveFormat, aliveCount);
        }
    }

    /// <summary>
    /// Update kill progress
    /// </summary>
    private void UpdateKillsText(int currentKills, int targetKills)
    {
        if (killsText != null)
        {
            killsText.text = string.Format(killsFormat, currentKills, targetKills);
        }
    }

    /// <summary>
    /// Start break countdown display
    /// </summary>
    private void StartBreakCountdown(float duration)
    {
        if (breakText != null)
        {
            isOnBreak = true;
            breakTimeRemaining = duration;
            breakText.gameObject.SetActive(true);
            breakText.text = string.Format(breakFormat, Mathf.CeilToInt(duration));
        }
    }
}
