using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Animated wave announcer that shows big text when waves start
/// </summary>
public class WaveAnnouncer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Large text for wave announcements")]
    public TextMeshProUGUI announcementText;

    [Tooltip("Optional panel background")]
    public GameObject announcementPanel;

    [Header("Animation")]
    [Tooltip("How long the announcement stays on screen")]
    [Min(0.5f)]
    public float displayDuration = 2f;

    [Tooltip("Fade in duration")]
    [Min(0.1f)]
    public float fadeInDuration = 0.3f;

    [Tooltip("Fade out duration")]
    [Min(0.1f)]
    public float fadeOutDuration = 0.5f;

    [Tooltip("Scale animation (punch effect)")]
    public bool useScaleAnimation = true;

    [Tooltip("Start scale for punch effect")]
    [Min(0.1f)]
    public float startScale = 0.5f;

    [Tooltip("End scale for punch effect")]
    [Min(0.1f)]
    public float endScale = 1.2f;

    [Header("Wave Messages")]
    [Tooltip("Format for normal waves")]
    public string waveFormat = "WAVE {0}";

    [Tooltip("Format for milestone waves (every 10)")]
    public string milestoneFormat = "WAVE {0}\n<size=60%>GET READY!</size>";

    [Tooltip("Format for boss waves")]
    public string bossFormat = "WAVE {0}\n<size=60%><color=red>BOSS INCOMING!</color></size>";

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color milestoneColor = Color.yellow;
    public Color bossColor = Color.red;

    [Header("Audio (Optional)")]
    public AudioClip waveStartSound;
    public AudioClip bossWaveSound;

    private WaveDirector waveDirector;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Coroutine currentAnimation;

    private void Awake()
    {
        // Setup canvas group for fading
        if (announcementText != null)
        {
            canvasGroup = announcementText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = announcementText.gameObject.AddComponent<CanvasGroup>();
            }

            rectTransform = announcementText.GetComponent<RectTransform>();
        }

        // Start hidden
        if (announcementPanel != null)
        {
            announcementPanel.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void Start()
    {
        waveDirector = FindObjectOfType<WaveDirector>();

        if (waveDirector == null)
        {
            Debug.LogWarning("WaveAnnouncer: No WaveDirector found!");
            enabled = false;
            return;
        }

        waveDirector.OnWaveStarted += HandleWaveStarted;
    }

    private void OnDestroy()
    {
        if (waveDirector != null)
        {
            waveDirector.OnWaveStarted -= HandleWaveStarted;
        }
    }

    private void HandleWaveStarted(int waveIndex)
    {
        // Determine if this is a special wave
        bool isMilestone = (waveIndex + 1) % 10 == 0; // Every 10th wave
        bool isBoss = CheckIfBossWave(waveIndex);

        // Format message
        string message;
        Color color;

        if (isBoss)
        {
            message = string.Format(bossFormat, waveIndex + 1);
            color = bossColor;

            // Play boss sound
            if (bossWaveSound != null && AudioManager.Instance != null)
            {
                // AudioManager.Instance.PlaySound(bossWaveSound);
            }
        }
        else if (isMilestone)
        {
            message = string.Format(milestoneFormat, waveIndex + 1);
            color = milestoneColor;
        }
        else
        {
            message = string.Format(waveFormat, waveIndex + 1);
            color = normalColor;
        }

        // Show announcement
        ShowAnnouncement(message, color);

        // Play normal sound
        if (!isBoss && waveStartSound != null && AudioManager.Instance != null)
        {
            // AudioManager.Instance.PlaySound(waveStartSound);
        }
    }

    private bool CheckIfBossWave(int waveIndex)
    {
        if (waveDirector == null) return false;

        WaveConfig currentWave = waveDirector.GetCurrentWave();
        if (currentWave == null) return false;

        return currentWave.bossPrefab != null &&
               (currentWave.spawnBossAtStart || currentWave.bossTimeSeconds > 0);
    }

    public void ShowAnnouncement(string message, Color color)
    {
        if (announcementText == null) return;

        // Stop any existing animation
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(AnimateAnnouncement(message, color));
    }

    private IEnumerator AnimateAnnouncement(string message, Color color)
    {
        // Show panel
        if (announcementPanel != null)
        {
            announcementPanel.SetActive(true);
        }

        // Set text and color
        announcementText.text = message;
        announcementText.color = color;

        // Reset scale
        if (useScaleAnimation && rectTransform != null)
        {
            rectTransform.localScale = Vector3.one * startScale;
        }

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;

            // Fade
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            }

            // Scale
            if (useScaleAnimation && rectTransform != null)
            {
                float scale = Mathf.Lerp(startScale, endScale, EaseOutBack(t));
                rectTransform.localScale = Vector3.one * scale;
            }

            yield return null;
        }

        // Ensure fully visible
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            }

            yield return null;
        }

        // Hide
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (announcementPanel != null)
        {
            announcementPanel.SetActive(false);
        }

        // Reset scale
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Ease out back function for bounce effect
    /// </summary>
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
