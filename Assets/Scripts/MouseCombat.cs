using UnityEngine;

/// <summary>
/// Auto-attack combat system like Vampire Survivors.
/// Automatically attacks in the character's facing direction.
/// Can auto-target nearest enemy or attack in movement direction.
/// </summary>
[RequireComponent(typeof(Animator))]
public class MouseCombat : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private float hitboxDistance = 1f;
    [SerializeField] private int damage = 10;

    [Header("Visual Effects")]
    [SerializeField] private bool enableSlashVFX = true;
    [SerializeField] private GameObject slashVFXPrefab;
    [SerializeField] private float vfxDuration = 0.5f;
    [SerializeField] private Vector3 vfxOffset = new Vector3(0.5f, 0f, 0f);
    [SerializeField] private float vfxScale = 1f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private float hitboxActiveDuration = 0.1f;
    [SerializeField] private bool enableAttackAnimation = true; // Re-enabled for movement attacking

    [Header("Auto-Attack Settings")]
    [SerializeField] private bool enableAutoAttack = true;
    [SerializeField] private float attacksPerSecond = 2f; // How many attacks per second
    [Tooltip("Auto-attack targets nearest enemy. If false, attacks in current facing direction.")]
    [SerializeField] private bool autoTargetNearestEnemy = true;
    [SerializeField] private float detectionRange = 10f; // Range to detect enemies

    private Animator animator;
    private GameObject currentHitbox;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Auto-attack system (like Vampire Survivors)
        if (enableAutoAttack && Time.time >= nextAttackTime)
        {
            if (autoTargetNearestEnemy)
            {
                // Find nearest enemy and face towards it before attacking
                GameObject nearestEnemy = FindNearestEnemy();
                if (nearestEnemy != null)
                {
                    // Set animator direction towards enemy
                    Vector3 directionToEnemy = nearestEnemy.transform.position - transform.position;
                    float facingX = directionToEnemy.x > 0 ? 1f : -1f;
                    animator.SetFloat("X", facingX);

                    PerformAttack();
                    nextAttackTime = Time.time + (1f / attacksPerSecond);
                }
            }
            else
            {
                // Attack in current facing direction automatically
                PerformAttack();
                nextAttackTime = Time.time + (1f / attacksPerSecond);
            }
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDistance = detectionRange;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void PerformAttack()
    {
        // Allow attacks to queue/overlap - don't check isAttacking
        // This allows attacking while moving

        // Attack in current facing direction (from animator)
        TriggerAttack();
    }

    private void TriggerAttack()
    {
        // Only trigger animation if enabled
        if (enableAttackAnimation)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        isAttacking = true;

        // Don't stop movement - allow attacking while moving

        // Spawn slash VFX
        if (enableSlashVFX)
        {
            SpawnSlashVFX();
        }

        // Activate weapon hitbox immediately
        ActivateHitbox();

        // Deactivate hitbox after short duration
        CancelInvoke(nameof(DeactivateHitbox));
        Invoke(nameof(DeactivateHitbox), hitboxActiveDuration);

        // End attack after full animation duration
        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), attackDuration);
    }

    private void SpawnSlashVFX()
    {
        if (slashVFXPrefab == null)
        {
            Debug.LogWarning("Slash VFX Prefab is not assigned!");
            return;
        }

        // Get current facing direction from animator
        float facingX = animator.GetFloat("X");

        // Calculate spawn position based on facing direction
        Vector3 playerPos = transform.position;
        Vector3 offsetDirection = new Vector3(vfxOffset.x * facingX, vfxOffset.y, vfxOffset.z);
        Vector3 spawnPos = playerPos + offsetDirection;

        // Spawn the slash VFX
        GameObject slashEffect = Instantiate(slashVFXPrefab, spawnPos, Quaternion.identity);

        // Flip the slash effect based on facing direction
        SpriteRenderer slashSprite = slashEffect.GetComponent<SpriteRenderer>();
        if (slashSprite != null)
        {
            slashSprite.flipX = facingX < 0; // Flip when facing left
            slashSprite.sortingOrder = 10; // Appear above player
        }

        // Scale the effect
        slashEffect.transform.localScale = new Vector3(vfxScale, vfxScale, 1f);

        // Destroy after duration
        Destroy(slashEffect, vfxDuration);

        Debug.Log($"Spawned slash VFX at {spawnPos}, facing: {(facingX > 0 ? "RIGHT" : "LEFT")}");
    }

    private void ActivateHitbox()
    {
        Debug.Log("=== ActivateHitbox called ===");

        // Play swing sound when attack starts
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySwingSound();
        }

        if (currentHitbox != null)
        {
            Debug.Log("Destroying existing hitbox");
            Destroy(currentHitbox);
        }

        if (hitboxPrefab == null)
        {
            Debug.LogError("Hitbox Prefab is NULL! Please assign it in the Inspector.");
            return;
        }

        // Get current facing direction from animator
        float facingX = animator.GetFloat("X");

        // Get hitbox size to properly position it
        BoxCollider2D prefabCollider = hitboxPrefab.GetComponent<BoxCollider2D>();
        float hitboxHalfWidth = prefabCollider != null ? prefabCollider.size.x / 2f : 0.5f;

        // Calculate spawn position in the direction the player is facing
        Vector3 playerPos = transform.position;
        float totalOffset = (hitboxHalfWidth + hitboxDistance) * facingX;
        Vector3 spawnPos = playerPos + new Vector3(totalOffset, 0f, 0f);

        Debug.Log($"Spawning hitbox at {spawnPos} (facing direction: {facingX})");

        // Spawn hitbox
        currentHitbox = Instantiate(hitboxPrefab, spawnPos, Quaternion.identity);

        // Set damage
        SwordHitbox hitbox = currentHitbox.GetComponent<SwordHitbox>();
        if (hitbox != null)
        {
            hitbox.SetDamage(damage);
            Debug.Log($"Damage set to: {damage}");
        }
        else
        {
            Debug.LogError("SwordHitbox component not found on instantiated prefab!");
        }
    }

    private void DeactivateHitbox()
    {
        Debug.Log("=== DeactivateHitbox called ===");

        if (currentHitbox == null)
        {
            Debug.Log("Hitbox already null, skipping deactivation");
            return;
        }

        Debug.Log($"Destroying hitbox at position: {currentHitbox.transform.position}");
        Destroy(currentHitbox);
        currentHitbox = null;
    }

    private void EndAttack()
    {
        isAttacking = false;

        // Don't need to re-enable movement since we never disabled it

        // Cancel any pending Invoke to prevent double-calling
        CancelInvoke(nameof(EndAttack));
        CancelInvoke(nameof(DeactivateHitbox));

        Debug.Log("Attack ended");
    }

    // Public method to check if currently attacking
    public bool IsAttacking()
    {
        return isAttacking;
    }

    // God mode support
    public float GetAttacksPerSecond() => attacksPerSecond;
    public void SetAttacksPerSecond(float value)
    {
        attacksPerSecond = value;
        Debug.Log($"Attacks per second set to: {value}");
    }
}
