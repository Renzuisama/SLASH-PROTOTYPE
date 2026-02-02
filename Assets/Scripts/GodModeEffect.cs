using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

/// <summary>
/// Handles God Mode buff on the player.
/// Press E to activate. Modifies special attack cooldown, dash stamina/speed, and attack speed.
/// </summary>
public class GodModeEffect : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private float godModeCooldown = 60f; // Cooldown before you can use god mode again

    [Header("God Mode Settings")]
    [SerializeField] private float godModeDuration = 15f;
    [SerializeField] private float godModeSpecialCooldown = 1f;
    [SerializeField] private float godModeDashSpeed = 30f;
    [SerializeField] private float godModeAttacksPerSecond = 3f;

    [Header("UI References")]
    [SerializeField] private GameObject godModeUIPanel;
    [SerializeField] private TextMeshProUGUI godModeTimerText;
    [SerializeField] private TextMeshProUGUI godModeCooldownText; // Shows cooldown when not active

    [Header("Visual Effects")]
    [SerializeField] private Color godModePlayerTint = new Color(1f, 0.8f, 0.2f, 1f); // Golden tint

    // Cached original values
    private float originalSpecialCooldown;
    private float originalDashSpeed;
    private float originalAttacksPerSecond;
    private bool originalDashUsesStamina;

    // Component references
    private SpecialAttack specialAttack;
    private PlayerMovement playerMovement;
    private PlayerStamina playerStamina;
    private MouseCombat mouseCombat;
    private SpriteRenderer spriteRenderer;

    // State
    private bool isGodModeActive = false;
    private bool canActivateGodMode = true;
    private float lastGodModeEndTime = -999f;
    private Coroutine godModeCoroutine;
    private Color originalPlayerColor;

    public bool IsGodModeActive => isGodModeActive;
    public bool CanActivateGodMode => canActivateGodMode && !isGodModeActive;

    private void Awake()
    {
        // Get component references
        specialAttack = GetComponent<SpecialAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStamina = GetComponent<PlayerStamina>();
        mouseCombat = GetComponent<MouseCombat>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalPlayerColor = spriteRenderer.color;
        }

        // Hide UI initially
        if (godModeUIPanel != null)
        {
            godModeUIPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Check for E key press to activate god mode
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryActivateGodMode();
        }

        // Update cooldown UI when not active
        UpdateCooldownUI();
    }

    private void TryActivateGodMode()
    {
        if (isGodModeActive)
        {
            Debug.Log("God Mode already active!");
            return;
        }

        if (!canActivateGodMode)
        {
            float remainingCooldown = GetRemainingCooldown();
            Debug.Log($"God Mode on cooldown! {remainingCooldown:F1}s remaining");
            return;
        }

        ActivateGodMode();
    }

    private void UpdateCooldownUI()
    {
        if (godModeCooldownText == null) return;

        if (isGodModeActive)
        {
            // Hide cooldown text during god mode (timer text shows instead)
            godModeCooldownText.gameObject.SetActive(false);
        }
        else if (!canActivateGodMode)
        {
            // Show cooldown with "God Mode: Xs" format
            float remainingCooldown = GetRemainingCooldown();
            godModeCooldownText.gameObject.SetActive(true);
            godModeCooldownText.text = $"God Mode: {Mathf.CeilToInt(remainingCooldown)}s";
            godModeCooldownText.color = Color.gray;

            // Check if cooldown is done
            if (remainingCooldown <= 0)
            {
                canActivateGodMode = true;
            }
        }
        else
        {
            // Ready to use - hide text
            godModeCooldownText.gameObject.SetActive(false);
        }
    }

    private float GetRemainingCooldown()
    {
        float elapsed = Time.time - lastGodModeEndTime;
        return Mathf.Max(0f, godModeCooldown - elapsed);
    }

    /// <summary>
    /// Activate God Mode for the specified duration
    /// </summary>
    public void ActivateGodMode()
    {
        if (godModeCoroutine != null)
        {
            // If already active, just extend the duration
            StopCoroutine(godModeCoroutine);
        }

        godModeCoroutine = StartCoroutine(GodModeRoutine());
    }

    private IEnumerator GodModeRoutine()
    {
        // Store original values and apply god mode
        ApplyGodMode();

        float remainingTime = godModeDuration;

        // Show UI
        if (godModeUIPanel != null)
        {
            godModeUIPanel.SetActive(true);
        }

        // Countdown
        while (remainingTime > 0)
        {
            // Update UI with simple format like "5s"
            if (godModeTimerText != null)
            {
                godModeTimerText.text = $"{Mathf.CeilToInt(remainingTime)}s";
            }

            yield return null;
            remainingTime -= Time.deltaTime;
        }

        // Remove god mode
        RemoveGodMode();

        // Hide UI
        if (godModeUIPanel != null)
        {
            godModeUIPanel.SetActive(false);
        }

        godModeCoroutine = null;
    }

    private void ApplyGodMode()
    {
        isGodModeActive = true;
        Debug.Log("=== GOD MODE ACTIVATED ===");

        // 1. Special Attack - reduce cooldown to 1 second
        if (specialAttack != null)
        {
            originalSpecialCooldown = specialAttack.GetCooldownTime();
            specialAttack.SetCooldownTime(godModeSpecialCooldown);
            Debug.Log($"Special cooldown: {originalSpecialCooldown}s -> {godModeSpecialCooldown}s");
        }

        // 2. Dash - no stamina cost and increased speed
        if (playerStamina != null)
        {
            originalDashUsesStamina = playerStamina.DashUsesStamina;
            playerStamina.SetDashUsesStamina(false);
            Debug.Log($"Dash stamina: {originalDashUsesStamina} -> false");
        }

        if (playerMovement != null)
        {
            originalDashSpeed = playerMovement.GetDashSpeed();
            playerMovement.SetDashSpeed(godModeDashSpeed);
            Debug.Log($"Dash speed: {originalDashSpeed} -> {godModeDashSpeed}");
        }

        // 3. Attack speed - increase to 3 per second
        if (mouseCombat != null)
        {
            originalAttacksPerSecond = mouseCombat.GetAttacksPerSecond();
            mouseCombat.SetAttacksPerSecond(godModeAttacksPerSecond);
            Debug.Log($"Attacks per second: {originalAttacksPerSecond} -> {godModeAttacksPerSecond}");
        }

        // Visual effect - tint player golden
        if (spriteRenderer != null)
        {
            spriteRenderer.color = godModePlayerTint;
        }
    }

    private void RemoveGodMode()
    {
        isGodModeActive = false;
        canActivateGodMode = false; // Start cooldown
        lastGodModeEndTime = Time.time;
        Debug.Log($"=== GOD MODE DEACTIVATED === Cooldown: {godModeCooldown}s");

        // Restore original values
        if (specialAttack != null)
        {
            specialAttack.SetCooldownTime(originalSpecialCooldown);
        }

        if (playerStamina != null)
        {
            playerStamina.SetDashUsesStamina(originalDashUsesStamina);
        }

        if (playerMovement != null)
        {
            playerMovement.SetDashSpeed(originalDashSpeed);
        }

        if (mouseCombat != null)
        {
            mouseCombat.SetAttacksPerSecond(originalAttacksPerSecond);
        }

        // Restore original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalPlayerColor;
        }
    }

    /// <summary>
    /// Get remaining god mode time
    /// </summary>
    public float GetRemainingTime()
    {
        // This is approximate - the actual time is tracked in the coroutine
        return isGodModeActive ? godModeDuration : 0f;
    }
}
