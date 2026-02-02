using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image fillImage;

    [Header("Colors")]
    [SerializeField] private Color fullStaminaColor = new Color(1f, 0.92f, 0.016f, 1f); // Bright yellow
    [SerializeField] private Color lowStaminaColor = new Color(0.8f, 0.6f, 0f, 1f); // Darker yellow/orange
    [SerializeField] private Color emptyStaminaColor = new Color(0.5f, 0.3f, 0f, 1f); // Dark orange

    [Header("Visual Settings")]
    [SerializeField] private bool smoothTransition = true;
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private bool colorGradient = true;

    private float targetStamina;
    private float maxStamina;

    private void Awake()
    {
        // Auto-find components if not assigned
        if (staminaSlider == null)
        {
            staminaSlider = GetComponent<Slider>();
        }

        if (fillImage == null && staminaSlider != null)
        {
            fillImage = staminaSlider.fillRect.GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (smoothTransition && staminaSlider != null)
        {
            // Smooth transition to target value
            staminaSlider.value = Mathf.Lerp(staminaSlider.value, targetStamina, Time.deltaTime * transitionSpeed);
        }
    }

    public void SetMaxStamina(float max)
    {
        maxStamina = max;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = max;
            staminaSlider.value = max;
        }

        targetStamina = max;
        UpdateColor(max / max);
    }

    public void SetStamina(float stamina)
    {
        targetStamina = stamina;

        if (!smoothTransition && staminaSlider != null)
        {
            staminaSlider.value = stamina;
        }

        // Update color based on stamina percentage
        if (maxStamina > 0)
        {
            UpdateColor(stamina / maxStamina);
        }
    }

    private void UpdateColor(float percentage)
    {
        if (!colorGradient || fillImage == null)
        {
            return;
        }

        // Gradient from empty -> low -> full
        Color targetColor;

        if (percentage > 0.5f)
        {
            // Lerp between low and full (50% to 100%)
            float t = (percentage - 0.5f) / 0.5f;
            targetColor = Color.Lerp(lowStaminaColor, fullStaminaColor, t);
        }
        else
        {
            // Lerp between empty and low (0% to 50%)
            float t = percentage / 0.5f;
            targetColor = Color.Lerp(emptyStaminaColor, lowStaminaColor, t);
        }

        fillImage.color = targetColor;
    }

    /// <summary>
    /// Flash the stamina bar (e.g., when trying to dash without stamina)
    /// </summary>
    public void FlashLowStamina()
    {
        if (fillImage != null)
        {
            StartCoroutine(FlashCoroutine());
        }
    }

    private System.Collections.IEnumerator FlashCoroutine()
    {
        Color originalColor = fillImage.color;

        // Flash red briefly
        fillImage.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        fillImage.color = originalColor;
        yield return new WaitForSeconds(0.1f);

        fillImage.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        fillImage.color = originalColor;
    }
}
