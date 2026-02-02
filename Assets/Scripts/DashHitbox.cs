using System.Collections.Generic;
using UnityEngine;

public class DashHitbox : MonoBehaviour
{
    [Header("Dash Damage Settings")]
    [SerializeField] private int dashDamage = 15;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.3f;

    [Header("Detection")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float detectionRadius = 1f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private bool isActive = false;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius in Scene view
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void ActivateDashHitbox()
    {
        isActive = true;
        hitEnemies.Clear();

        if (debugMode)
        {
            Debug.Log("[Dash Hitbox] Activated");
        }
    }

    public void DeactivateDashHitbox()
    {
        isActive = false;
        hitEnemies.Clear();

        if (debugMode)
        {
            Debug.Log("[Dash Hitbox] Deactivated");
        }
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        // Check for enemies in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            // Skip if we already hit this enemy during this dash
            if (hitEnemies.Contains(hit.gameObject))
            {
                continue;
            }

            // Try to damage the enemy
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Deal damage
                damageable.TakeDamage(dashDamage);
                hitEnemies.Add(hit.gameObject);

                if (debugMode)
                {
                    Debug.Log($"[Dash Hitbox] Hit {hit.gameObject.name} for {dashDamage} damage");
                }

                // Apply knockback
                ApplyKnockback(hit.gameObject);

                // Show damage text
                if (DamageTextManager.Instance != null)
                {
                    DamageTextManager.Instance.ShowDamage(hit.transform.position, dashDamage, false);
                }
            }
        }
    }

    private void ApplyKnockback(GameObject enemy)
    {
        // Calculate knockback direction (away from player)
        Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;

        // Try to apply knockback to Rigidbody2D
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            // Apply force-based knockback
            enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

            if (debugMode)
            {
                Debug.Log($"[Dash Hitbox] Applied knockback to {enemy.name} in direction {knockbackDirection}");
            }

            // Apply stun effect to disable enemy movement temporarily
            EnemyAnimated enemyAnimated = enemy.GetComponent<EnemyAnimated>();
            if (enemyAnimated != null)
            {
                enemyAnimated.ApplyStun(knockbackDuration);
            }
        }
    }

    // Public getters for settings (in case you want to modify them at runtime)
    public void SetDashDamage(int damage)
    {
        dashDamage = damage;
    }

    public void SetKnockbackForce(float force)
    {
        knockbackForce = force;
    }

    public int GetDashDamage()
    {
        return dashDamage;
    }

    public float GetKnockbackForce()
    {
        return knockbackForce;
    }
}
