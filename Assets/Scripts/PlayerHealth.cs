using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private HealthBar healthBar;

    [Header("Damage Animation")]
    [SerializeField] private float hurtDuration = 0.5f; // Duration of hurt animation

    private Animator animator;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private bool isHurt = false;
    private bool isDead = false;

    // Animator parameter
    private static readonly int Hurt = Animator.StringToHash("Hurt");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        Debug.Log($"Player health initialized: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int damage)
    {
        if (isHurt || isDead) return; // Already taking damage or dead, ignore

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");

        // Show damage text in red above player
        if (DamageTextManager.Instance != null)
        {
            DamageTextManager.Instance.ShowDamage(transform.position, damage, false, Color.red);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth);
            }
            HandleDeath();
        }
        else
        {
            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth);
            }
            StartCoroutine(PlayHurtAnimation());
        }
    }

    private IEnumerator PlayHurtAnimation()
    {
        isHurt = true;

        // Disable player movement during hurt
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        // Trigger hurt animation
        animator.SetTrigger(Hurt);

        // Wait for hurt animation to complete
        yield return new WaitForSeconds(hurtDuration);

        // Re-enable movement
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }

        isHurt = false;
    }

    private void HandleDeath()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player has died!");

        // Disable player controls
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        // Start death sequence
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Optional: Play death animation if you have one
        // animator.SetTrigger("Die");

        // Fade out the player
        if (spriteRenderer != null)
        {
            float fadeDuration = 1.5f;
            float elapsed = 0f;
            Color startColor = spriteRenderer.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            // Ensure fully transparent
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        }

        // Wait a moment
        yield return new WaitForSeconds(0.5f);

        // Disable the player GameObject
        gameObject.SetActive(false);

        Debug.Log("Game Over! Player has been removed.");

        // TODO: Show game over screen or restart scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene(
        //     UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        // );
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log($"Player healed {amount}. Health: {currentHealth}/{maxHealth}");
    }
}
