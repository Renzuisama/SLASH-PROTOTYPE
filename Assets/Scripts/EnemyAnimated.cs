using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyAnimated : MonoBehaviour, IDamageable, IPoolable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;
    private int baseMaxHealth; // Store original max health

    [Header("Chase")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stopDistance = 0.6f;
    [SerializeField] private string playerTag = "Player";
    private float baseMoveSpeed; // Store original move speed

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.2f; // Slightly larger than stopDistance
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 5;
    private float lastAttackTime = -999f;
    private int baseAttackDamage; // Store original attack damage

    [Header("Animation")]
    [SerializeField] private float takeDamageDuration = 0.5f; // Duration to pause when hit
    [SerializeField] float deathAnimLength = 1.5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDir;

    private bool isDead = false;
    private bool isTakingDamage = false;
    private bool isAttacking = false;
    private bool canDealDamage = false; // Flag to control when enemy can damage player

    // Animator parameter name constants
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int DieParam = Animator.StringToHash("Die");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Store base values in Awake so they're available before Start
        // This is critical for difficulty scaling which happens right after Instantiate
        baseMaxHealth = maxHealth;
        baseMoveSpeed = moveSpeed;
        baseAttackDamage = attackDamage;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) player = p.transform;
        else Debug.LogWarning($"Enemy: No GameObject found with tag '{playerTag}'.");
    }

    private void Update()
    {
        if (isDead || isTakingDamage || isAttacking) return;

        if (player == null)
        {
            moveDir = Vector2.zero;
            animator.SetBool(IsMoving, false);
            return;
        }

        Vector2 toPlayer = (Vector2)(player.position - transform.position);
        float dist = toPlayer.magnitude;

        // Check if in attack range
        if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformAttack());
            moveDir = Vector2.zero;
            animator.SetBool(IsMoving, false);
            return;
        }

        // Check if should stop (within stopDistance but outside attack range)
        if (dist <= stopDistance)
        {
            moveDir = Vector2.zero;
            animator.SetBool(IsMoving, false);
            return;
        }

        // Chase player
        moveDir = toPlayer.normalized;
        animator.SetBool(IsMoving, true);
    }

    private void FixedUpdate()
    {
        if (isDead || isTakingDamage || isAttacking || moveDir == Vector2.zero) return;

        Vector2 newPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger(Attack);
        Debug.Log("Enemy attacked!");

        // Wait for attack animation to complete (adjust based on your animation length)
        yield return new WaitForSeconds(0.5f); // Typical attack animation duration

        isAttacking = false;
        canDealDamage = false; // Reset damage flag after attack
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // Note: Damage text is shown by the weapon that dealt the damage (e.g., SwordHitbox)
        // to ensure proper critical hit indication

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
        else
        {
            StartCoroutine(PlayTakeDamageAnimation());
        }
    }

    private IEnumerator PlayTakeDamageAnimation()
    {
        isTakingDamage = true;
        animator.SetTrigger(Hurt);

        yield return new WaitForSeconds(takeDamageDuration);

        isTakingDamage = false;
    }

    private void HandleDeath()
    {
        if (isDead) return;

        isDead = true;
        moveDir = Vector2.zero;

        Debug.Log($"{gameObject.name} has died!");
        animator.SetTrigger(DieParam);

        // Broadcast death event for wave system
        EnemyDeathBroadcaster deathBroadcaster = GetComponent<EnemyDeathBroadcaster>();
        if (deathBroadcaster != null)
        {
            deathBroadcaster.BroadcastDeath();
        }

        // Play death sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDeathSound();
        }

        // Disable movement and collision
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        // Destroy after death animation completes
        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        // Get the death animation clip length
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
         // Default fallback

        foreach (var clip in clipInfo)
        {
            if (clip.clip.name.Contains("death"))
            {
                deathAnimLength = clip.clip.length;
                break;
            }
        }

        // Wait for animation to finish
        yield return new WaitForSeconds(deathAnimLength);

        Destroy(gameObject);
    }

    // Optional: Animation Event called at the end of death animation
    // More reliable than coroutine timing
    public void OnDeathAnimationComplete()
    {
        Destroy(gameObject);
    }

    // Optional: Animation Event called during attack animation
    // Use this to trigger damage to player at the right moment
    public void OnAttackHit()
    {
        Debug.Log("Enemy attack hit event!");

        // Enable damage dealing during the attack hit window
        canDealDamage = true;
    }

    // Called when enemy collider overlaps with player (since Is Trigger is checked)
    private void OnTriggerStay2D(Collider2D other)
    {
        // Only deal damage if we're attacking and the hit frame has occurred
        if (!canDealDamage) return;
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                Debug.Log($"Enemy dealt {attackDamage} damage to player!");

                // Disable damage flag to prevent multiple hits in one attack
                canDealDamage = false;
            }
            else
            {
                Debug.LogWarning("Player doesn't have IDamageable component!");
            }
        }
    }

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // IPoolable implementation
    public void OnSpawnFromPool()
    {
        // Reset all state when respawned from pool
        currentHealth = maxHealth;
        isDead = false;
        isTakingDamage = false;
        isAttacking = false;
        canDealDamage = false;
        moveDir = Vector2.zero;
        lastAttackTime = -999f;

        // Re-enable components
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        // Reset animator
        if (animator != null)
        {
            animator.SetBool(IsMoving, false);
            animator.ResetTrigger(Attack);
            animator.ResetTrigger(Hurt);
            animator.ResetTrigger(DieParam);
        }

        // Find player again (in case it changed)
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }
    }

    public void OnReturnToPool()
    {
        // Clean up when returned to pool
        StopAllCoroutines();
        moveDir = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Apply difficulty scaling from wave manager
    /// </summary>
    public void ApplyDifficultyScaling(float hpMultiplier, float damageMultiplier, float speedMultiplier)
    {
        // Safety check: ensure base values are initialized
        // This should always be true since Awake runs before this is called
        if (baseMaxHealth == 0)
        {
            baseMaxHealth = maxHealth;
            baseMoveSpeed = moveSpeed;
            baseAttackDamage = attackDamage;
            Debug.LogWarning($"[{gameObject.name}] Base values were not initialized in Awake! Using current values as base.");
        }

        // Apply HP scaling
        maxHealth = Mathf.RoundToInt(baseMaxHealth * hpMultiplier);
        currentHealth = maxHealth;

        // Apply damage scaling
        attackDamage = Mathf.RoundToInt(baseAttackDamage * damageMultiplier);

        // Apply speed scaling
        moveSpeed = baseMoveSpeed * speedMultiplier;
    }

    /// <summary>
    /// Apply stun effect to temporarily disable enemy movement
    /// Called when enemy is hit by dash or other knockback effects
    /// </summary>
    public void ApplyStun(float stunDuration)
    {
        if (isDead) return;

        // If already stunned, don't start a new stun coroutine
        if (!isTakingDamage)
        {
            StartCoroutine(StunCoroutine(stunDuration));
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        bool wasAlreadyTakingDamage = isTakingDamage;
        isTakingDamage = true;
        moveDir = Vector2.zero;
        animator.SetBool(IsMoving, false);

        yield return new WaitForSeconds(duration);

        // Only restore if we set it
        if (!wasAlreadyTakingDamage)
        {
            isTakingDamage = false;
        }
    }
}
