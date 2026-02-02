using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;

    [Header("Stamina Costs")]
    [SerializeField] private float dashStaminaCost = 50f; // Cost for one dash (50 = 2 dashes max)
    [SerializeField] private float attackStaminaCost = 0f; // Optional: cost for attacking

    [Header("Regeneration")]
    [SerializeField] private float staminaRegenRate = 20f; // Stamina per second
    [SerializeField] private float regenDelay = 1f; // Delay before regen starts after action
    [SerializeField] private bool canRegenWhileMoving = true;

    [Header("References")]
    [SerializeField] private StaminaBar staminaBar;

    private float lastActionTime;
    private bool isRegenerating = false;

    // Public properties
    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;

    // God mode support
    private bool dashUsesStamina = true;
    public bool DashUsesStamina => dashUsesStamina;

    public void SetDashUsesStamina(bool value)
    {
        dashUsesStamina = value;
        Debug.Log($"Dash uses stamina set to: {value}");
    }

    private void Start()
    {
        currentStamina = maxStamina;

        if (staminaBar != null)
        {
            staminaBar.SetMaxStamina(maxStamina);
            staminaBar.SetStamina(currentStamina);
        }

        Debug.Log($"Player stamina initialized: {currentStamina}/{maxStamina}");
    }

    private void Update()
    {
        // Check if enough time has passed since last action to start regenerating
        if (Time.time >= lastActionTime + regenDelay)
        {
            RegenerateStamina();
        }
    }

    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            if (!isRegenerating)
            {
                isRegenerating = true;
            }

            // Regenerate stamina over time
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);

            // Update UI
            if (staminaBar != null)
            {
                staminaBar.SetStamina(currentStamina);
            }
        }
        else if (isRegenerating)
        {
            isRegenerating = false;
            currentStamina = maxStamina; // Ensure it's exactly max

            if (staminaBar != null)
            {
                staminaBar.SetStamina(currentStamina);
            }
        }
    }

    /// <summary>
    /// Try to consume stamina for dash. Returns true if successful.
    /// </summary>
    public bool TryConsumeDashStamina()
    {
        // God mode - no stamina cost
        if (!dashUsesStamina)
        {
            Debug.Log("God Mode: Dash costs no stamina!");
            return true;
        }

        if (currentStamina >= dashStaminaCost)
        {
            currentStamina -= dashStaminaCost;
            lastActionTime = Time.time;
            isRegenerating = false;

            if (staminaBar != null)
            {
                staminaBar.SetStamina(currentStamina);
            }

            Debug.Log($"Dash consumed {dashStaminaCost} stamina. Remaining: {currentStamina}/{maxStamina}");
            return true;
        }
        else
        {
            Debug.Log("Not enough stamina to dash!");
            return false;
        }
    }

    /// <summary>
    /// Try to consume stamina for attack. Returns true if successful.
    /// </summary>
    public bool TryConsumeAttackStamina()
    {
        if (attackStaminaCost <= 0)
        {
            return true; // No stamina cost for attacks
        }

        if (currentStamina >= attackStaminaCost)
        {
            currentStamina -= attackStaminaCost;
            lastActionTime = Time.time;
            isRegenerating = false;

            if (staminaBar != null)
            {
                staminaBar.SetStamina(currentStamina);
            }

            Debug.Log($"Attack consumed {attackStaminaCost} stamina. Remaining: {currentStamina}/{maxStamina}");
            return true;
        }
        else
        {
            Debug.Log("Not enough stamina to attack!");
            return false;
        }
    }

    /// <summary>
    /// Check if player has enough stamina for dash
    /// </summary>
    public bool HasEnoughStaminaForDash()
    {
        // God mode - always have enough stamina
        if (!dashUsesStamina) return true;
        return currentStamina >= dashStaminaCost;
    }

    /// <summary>
    /// Check if player has enough stamina for attack
    /// </summary>
    public bool HasEnoughStaminaForAttack()
    {
        return attackStaminaCost <= 0 || currentStamina >= attackStaminaCost;
    }

    /// <summary>
    /// Force stop regeneration (called when action is performed)
    /// </summary>
    public void StopRegeneration()
    {
        lastActionTime = Time.time;
        isRegenerating = false;
    }

    /// <summary>
    /// Get stamina percentage (0-1)
    /// </summary>
    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }

    /// <summary>
    /// Restore stamina (for pickups, rest points, etc.)
    /// </summary>
    public void RestoreStamina(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);

        if (staminaBar != null)
        {
            staminaBar.SetStamina(currentStamina);
        }

        Debug.Log($"Restored {amount} stamina. Current: {currentStamina}/{maxStamina}");
    }

    /// <summary>
    /// Fully restore stamina
    /// </summary>
    public void FullyRestoreStamina()
    {
        currentStamina = maxStamina;

        if (staminaBar != null)
        {
            staminaBar.SetStamina(currentStamina);
        }

        Debug.Log("Stamina fully restored!");
    }

    /// <summary>
    /// Consume stamina (for special attacks, etc.)
    /// Returns true if successful
    /// </summary>
    public bool ConsumeStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            lastActionTime = Time.time;
            isRegenerating = false;

            if (staminaBar != null)
            {
                staminaBar.SetStamina(currentStamina);
            }

            Debug.Log($"Consumed {amount} stamina. Remaining: {currentStamina}/{maxStamina}");
            return true;
        }
        else
        {
            Debug.Log($"Not enough stamina! Need {amount}, have {currentStamina}");
            return false;
        }
    }
}
