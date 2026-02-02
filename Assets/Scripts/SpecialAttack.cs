using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialAttack : MonoBehaviour
{
    [Header("Holy Cross Special Attack")]
    [SerializeField] private GameObject holyCrossPrefab;
    [SerializeField] private int specialDamage = 50;
    [SerializeField] private float aoeRadius = 5f;
    [SerializeField] private float attackDuration = 2f;

    [Header("Stamina/Cooldown")]
    [SerializeField] private float specialStaminaCost = 100f; // Full stamina bar
    [SerializeField] private float cooldownTime = 10f;
    [SerializeField] private bool useStamina = false; // Disabled stamina requirement
    [SerializeField] private bool useCooldown = true;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float detectionRange = 15f; // How far to look for enemies
    [SerializeField] private bool targetClosestEnemy = true;

    [Header("Visual Effects")]
    [SerializeField] private bool showAOERadius = true;
    [SerializeField] private Color aoeGizmoColor = Color.yellow;

    [Header("Audio")]
    [SerializeField] private AudioClip specialAttackSound;

    [Header("Camera Shake")]
    [SerializeField] private bool enableCameraShake = true;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.3f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private PlayerStamina playerStamina;
    private CameraShake cameraShake;
    private bool canUseSpecial = true;
    private float lastSpecialTime = -999f;
    private AudioSource audioSource;

    private void Awake()
    {
        playerStamina = GetComponent<PlayerStamina>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Find camera shake component on main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraShake = mainCamera.GetComponent<CameraShake>();
            if (cameraShake == null && enableCameraShake)
            {
                Debug.LogWarning("CameraShake component not found on Main Camera. Camera shake will be disabled.");
            }
        }
    }

    private void Update()
    {
        // Check for right mouse button press
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryUseSpecialAttack();
        }
    }

    private void TryUseSpecialAttack()
    {
        // Check cooldown
        if (useCooldown && Time.time < lastSpecialTime + cooldownTime)
        {
            float remainingCooldown = (lastSpecialTime + cooldownTime) - Time.time;
            Debug.Log($"Special attack on cooldown! {remainingCooldown:F1}s remaining");
            return;
        }

        // Check stamina
        if (useStamina && playerStamina != null)
        {
            if (playerStamina.CurrentStamina < specialStaminaCost)
            {
                Debug.Log("Not enough stamina for special attack!");
                return;
            }
        }

        // Find target enemy
        GameObject targetEnemy = FindTargetEnemy();

        if (targetEnemy == null)
        {
            Debug.Log("No enemies in range for special attack!");
            return;
        }

        // Execute special attack
        ExecuteSpecialAttack(targetEnemy.transform.position);
    }

    private GameObject FindTargetEnemy()
    {
        // Find all enemies in detection range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);

        if (enemiesInRange.Length == 0)
        {
            return null;
        }

        if (targetClosestEnemy)
        {
            // Find closest enemy
            GameObject closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D enemyCollider in enemiesInRange)
            {
                float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemyCollider.gameObject;
                }
            }

            return closestEnemy;
        }
        else
        {
            // Return random enemy
            int randomIndex = Random.Range(0, enemiesInRange.Length);
            return enemiesInRange[randomIndex].gameObject;
        }
    }

    private void ExecuteSpecialAttack(Vector3 targetPosition)
    {
        // Consume stamina
        if (useStamina && playerStamina != null)
        {
            playerStamina.ConsumeStamina(specialStaminaCost);
        }

        // Set last use time
        lastSpecialTime = Time.time;

        // Spawn Holy Cross VFX at target position (centered on enemy)
        if (holyCrossPrefab != null)
        {
            // Offset position to center on enemy (raise it up a bit)
            Vector3 centeredPosition = targetPosition + new Vector3(0f, 0.5f, 0f);

            GameObject holyCross = Instantiate(holyCrossPrefab, centeredPosition, Quaternion.identity);

            // Check if it's a Canvas-based UI effect
            Canvas canvas = holyCross.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Convert to World Space canvas so it appears in the game world
                canvas.renderMode = RenderMode.WorldSpace;

                // Position it at the centered location
                holyCross.transform.position = centeredPosition;

                // Scale it appropriately for world space (UI is usually very large)
                holyCross.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

                // Set sorting order so it appears above enemies
                canvas.sortingLayerName = "Default";
                canvas.sortingOrder = 100;

                if (debugMode)
                {
                    Debug.Log($"Spawned Holy Cross (World Space Canvas) at {centeredPosition}");
                }
            }
            else
            {
                // Sprite-based effect - don't override scale, let prefab control it
                // The HolyCrossAnimator component already sets the scale
                if (debugMode)
                {
                    Debug.Log($"Spawned Holy Cross (Sprite) at {centeredPosition}, using prefab scale");
                }
            }

            // Destroy VFX after duration
            Destroy(holyCross, attackDuration);
        }
        else
        {
            Debug.LogError("Holy Cross prefab is not assigned!");
        }

        // Play sound
        if (specialAttackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(specialAttackSound);
        }

        // Trigger camera shake
        if (enableCameraShake && cameraShake != null)
        {
            cameraShake.Shake(shakeDuration, shakeMagnitude);

            if (debugMode)
            {
                Debug.Log($"Camera shake triggered - Duration: {shakeDuration}s, Magnitude: {shakeMagnitude}");
            }
        }

        // Start damage coroutine
        StartCoroutine(DealAOEDamage(targetPosition));

        if (debugMode)
        {
            Debug.Log($"Special attack executed at {targetPosition}! Damage: {specialDamage}, AOE Radius: {aoeRadius}");
        }
    }

    private IEnumerator DealAOEDamage(Vector3 center)
    {
        // Small delay to sync with VFX
        yield return new WaitForSeconds(0.2f);

        // Find all enemies in AOE radius
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(center, aoeRadius, enemyLayer);

        if (debugMode)
        {
            Debug.Log($"Holy Cross hit {enemiesHit.Length} enemies in AOE");
        }

        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

        foreach (Collider2D enemyCollider in enemiesHit)
        {
            GameObject enemy = enemyCollider.gameObject;

            // Prevent hitting same enemy multiple times
            if (damagedEnemies.Contains(enemy))
            {
                continue;
            }

            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(specialDamage);
                damagedEnemies.Add(enemy);

                // Show damage text
                if (DamageTextManager.Instance != null)
                {
                    DamageTextManager.Instance.ShowDamage(
                        enemy.transform.position,
                        specialDamage,
                        false,
                        Color.yellow // Yellow for special attack
                    );
                }

                if (debugMode)
                {
                    Debug.Log($"Holy Cross damaged {enemy.name} for {specialDamage}");
                }
            }
        }
    }

    /// <summary>
    /// Check if special attack is ready to use
    /// </summary>
    public bool IsSpecialReady()
    {
        if (useCooldown && Time.time < lastSpecialTime + cooldownTime)
        {
            return false;
        }

        if (useStamina && playerStamina != null)
        {
            return playerStamina.CurrentStamina >= specialStaminaCost;
        }

        return true;
    }

    /// <summary>
    /// Get remaining cooldown time
    /// </summary>
    public float GetRemainingCooldown()
    {
        if (!useCooldown) return 0f;

        float remaining = (lastSpecialTime + cooldownTime) - Time.time;
        return Mathf.Max(0f, remaining);
    }

    /// <summary>
    /// Get cooldown percentage (0-1)
    /// </summary>
    public float GetCooldownPercentage()
    {
        if (!useCooldown) return 1f;

        float elapsed = Time.time - lastSpecialTime;
        return Mathf.Clamp01(elapsed / cooldownTime);
    }

    // God mode support
    public float GetCooldownTime() => cooldownTime;
    public void SetCooldownTime(float value)
    {
        cooldownTime = value;
        Debug.Log($"Special attack cooldown set to: {value}s");
    }

    private void OnDrawGizmosSelected()
    {
        if (showAOERadius)
        {
            // Draw detection range
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Draw AOE damage radius at player position (for reference)
            Gizmos.color = aoeGizmoColor;
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
    }
}
