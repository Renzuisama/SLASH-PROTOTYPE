using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Vector2 input;
    private Rigidbody2D rb;
    private Animator animator;
    private WeaponController weaponController;
    private DashHitbox dashHitbox;
    private PlayerStamina playerStamina;
    private bool isAttacking;
    private bool canMove = true;
    private bool attackButtonHeld = false;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector2 dashDirection;

    private float lastFacingX = 1f; // +1 right, -1 left

    public float GetFacingX() => lastFacingX;

    // God mode support
    public float GetDashSpeed() => dashSpeed;
    public void SetDashSpeed(float value)
    {
        dashSpeed = value;
        Debug.Log($"Dash speed set to: {value}");
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            // Stop movement immediately
            input = Vector2.zero;
            animator.SetBool("IsWalking", false);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weaponController = GetComponent<WeaponController>();
        dashHitbox = GetComponent<DashHitbox>();
        playerStamina = GetComponent<PlayerStamina>();
        animator.SetFloat("X", lastFacingX);
    }

    private void OnMovement(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    private void Update()
    {
        if (!canMove && !isDashing) return;

        // Handle dash input
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && canDash && !isDashing && !isAttacking)
        {
            // Check if player has enough stamina to dash
            if (playerStamina != null && !playerStamina.HasEnoughStaminaForDash())
            {
                Debug.Log("Not enough stamina to dash!");
                return;
            }

            // Determine dash direction
            if (input.sqrMagnitude > 0.001f)
            {
                // Dash in movement direction
                dashDirection = input.normalized;
            }
            else
            {
                // Dash in facing direction if standing still
                dashDirection = new Vector2(lastFacingX, 0f);
            }
            StartCoroutine(PerformDash());
        }

        if (isDashing) return; // Skip normal movement/attack during dash

        bool isWalking = input.sqrMagnitude > 0.001f;
        animator.SetBool("IsWalking", isWalking);

        if (Mathf.Abs(input.x) > 0.01f)
        {
            float newFacingX = Mathf.Sign(input.x);
            if (newFacingX != lastFacingX)
            {
                Debug.Log($"Player facing direction changed from {lastFacingX} to {newFacingX}");
            }
            lastFacingX = newFacingX;
        }

        animator.SetFloat("X", lastFacingX);
        animator.SetFloat("Y", input.y);

        // DISABLED: MouseCombat handles all attacks now
        /*
        // Failsafe: Also check actual mouse button state
        if (!Mouse.current.rightButton.isPressed)
        {
            attackButtonHeld = false;
        }

        // Check for held attack button and trigger attacks continuously
        if (attackButtonHeld && !isAttacking && canMove)
        {
            // Don't attack while moving
            if (input.sqrMagnitude > 0.001f)
            {
                return;
            }

            TriggerAttack();
        }
        */
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // Dash movement
            float dt = Time.fixedDeltaTime;
            Vector2 nextPos = rb.position + dashDirection * (dashSpeed * dt);
            rb.MovePosition(nextPos);
        }
        else if (canMove)
        {
            // Normal movement
            float dt = Time.fixedDeltaTime;
            Vector2 nextPos = rb.position + input.normalized * (speed * dt);
            rb.MovePosition(nextPos);
        }
    }

    private void OnAttack(InputValue value)
    {
        // DISABLED: MouseCombat handles all attacks now
        // Track button state for hold-to-attack
        attackButtonHeld = value.isPressed;
        Debug.Log($"Attack input disabled - using MouseCombat auto-attack system");

        // Old attack system disabled
        /*
        if (value.isPressed && !isAttacking && canMove)
        {
            // Don't attack while moving
            if (input.sqrMagnitude > 0.001f)
            {
                animator.ResetTrigger("Attack");
                return;
            }

            TriggerAttack();
        }
        */
    }

    private void TriggerAttack()
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
        isAttacking = true;

        // Activate weapon hitbox immediately
        if (weaponController != null)
        {
            weaponController.ActivateHitbox();
        }

        // Deactivate hitbox after 0.1 seconds
        CancelInvoke(nameof(DeactivateHitbox));
        Invoke(nameof(DeactivateHitbox), 0.1f);

        // Schedule EndAttack after full animation duration (0.4s)
        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), 0.4f);
    }

    // Deactivate hitbox early (called after 0.1s)
    private void DeactivateHitbox()
    {
        if (weaponController != null)
        {
            weaponController.DeactivateHitbox();
        }
    }

    // Called by Animation Event at the end of the attack animation (or by Invoke as fallback)
    public void EndAttack()
    {
        isAttacking = false;

        // Cancel any pending Invoke to prevent double-calling
        CancelInvoke(nameof(EndAttack));
        CancelInvoke(nameof(DeactivateHitbox));
    }

    private System.Collections.IEnumerator PerformDash()
    {
        // Consume stamina for dash
        if (playerStamina != null)
        {
            if (!playerStamina.TryConsumeDashStamina())
            {
                Debug.Log("Failed to consume stamina for dash!");
                yield break;
            }
        }

        isDashing = true;
        canDash = false;

        // Play dash sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDashSound();
        }

        // Activate dash hitbox for damage
        if (dashHitbox != null)
        {
            dashHitbox.ActivateDashHitbox();
        }

        // Trigger dash animation
        animator.SetTrigger("Dash");
        animator.SetBool("IsDashing", true);

        Debug.Log($"Dashing in direction: {dashDirection}");

        // Perform dash for specified duration
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("IsDashing", false);

        // Deactivate dash hitbox
        if (dashHitbox != null)
        {
            dashHitbox.DeactivateDashHitbox();
        }

        // Wait for cooldown
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
        Debug.Log("Dash ready!");
    }
}
