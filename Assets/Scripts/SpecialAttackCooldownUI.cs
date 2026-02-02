using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialAttackCooldownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpecialAttack specialAttack;
    [SerializeField] private Image cooldownIcon;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [Header("Cooldown Display Type")]
    [SerializeField] private CooldownType cooldownType = CooldownType.Radial;

    [Header("Visual Settings")]
    [SerializeField] private Color readyColor = Color.white;
    [SerializeField] private Color cooldownColor = Color.gray;
    [SerializeField] private bool showCooldownText = true;
    [SerializeField] private bool showReadyText = true;
    [SerializeField] private string readyTextMessage = "READY (E)";

    [Header("Animation")]
    [SerializeField] private bool pulseWhenReady = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScale = 1.1f;

    private Vector3 originalScale;
    private bool wasReady = false;

    public enum CooldownType
    {
        Radial,      // 360-degree radial fill
        Vertical,    // Bottom-to-top vertical fill
        Horizontal   // Left-to-right horizontal fill
    }

    private void Awake()
    {
        // Auto-find SpecialAttack if not assigned
        if (specialAttack == null)
        {
            specialAttack = FindFirstObjectByType<SpecialAttack>();
        }

        if (cooldownFill != null)
        {
            originalScale = cooldownFill.transform.localScale;

            // Set fill type based on cooldown type
            switch (cooldownType)
            {
                case CooldownType.Radial:
                    cooldownFill.type = Image.Type.Filled;
                    cooldownFill.fillMethod = Image.FillMethod.Radial360;
                    cooldownFill.fillOrigin = (int)Image.Origin360.Top;
                    cooldownFill.fillClockwise = true;
                    break;

                case CooldownType.Vertical:
                    cooldownFill.type = Image.Type.Filled;
                    cooldownFill.fillMethod = Image.FillMethod.Vertical;
                    cooldownFill.fillOrigin = (int)Image.OriginVertical.Bottom;
                    break;

                case CooldownType.Horizontal:
                    cooldownFill.type = Image.Type.Filled;
                    cooldownFill.fillMethod = Image.FillMethod.Horizontal;
                    cooldownFill.fillOrigin = (int)Image.OriginHorizontal.Left;
                    break;
            }
        }
    }

    private void Update()
    {
        if (specialAttack == null) return;

        UpdateCooldownDisplay();
    }

    private void UpdateCooldownDisplay()
    {
        bool isReady = specialAttack.IsSpecialReady();
        float cooldownPercentage = specialAttack.GetCooldownPercentage();
        float remainingCooldown = specialAttack.GetRemainingCooldown();

        // Update fill amount (0 = on cooldown, 1 = ready)
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = cooldownPercentage;
        }

        // Update colors
        if (isReady)
        {
            // Ready to use
            if (cooldownIcon != null)
            {
                cooldownIcon.color = readyColor;
            }
            if (cooldownFill != null)
            {
                cooldownFill.color = readyColor;
            }

            // Show ready text
            if (cooldownText != null && showReadyText)
            {
                cooldownText.text = readyTextMessage;
                cooldownText.color = readyColor;
            }

            // Pulse animation when ready
            if (pulseWhenReady && cooldownIcon != null)
            {
                float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f);
                cooldownIcon.transform.localScale = originalScale * pulse;
            }

            wasReady = true;
        }
        else
        {
            // On cooldown
            if (cooldownIcon != null)
            {
                cooldownIcon.color = cooldownColor;
            }
            if (cooldownFill != null)
            {
                cooldownFill.color = cooldownColor;
            }

            // Show cooldown time
            if (cooldownText != null && showCooldownText)
            {
                cooldownText.text = $"{remainingCooldown:F1}s";
                cooldownText.color = cooldownColor;
            }

            // Reset scale
            if (cooldownIcon != null)
            {
                cooldownIcon.transform.localScale = originalScale;
            }

            wasReady = false;
        }
    }

    /// <summary>
    /// Change cooldown display type at runtime
    /// </summary>
    public void SetCooldownType(CooldownType newType)
    {
        cooldownType = newType;
        Awake(); // Re-initialize with new type
    }

    /// <summary>
    /// Set custom icon sprite
    /// </summary>
    public void SetIconSprite(Sprite sprite)
    {
        if (cooldownIcon != null)
        {
            cooldownIcon.sprite = sprite;
        }
    }
}
