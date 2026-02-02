using UnityEngine;

/// <summary>
/// Handles slash VFX spawning and animation for attacks.
/// Automatically flips the slash effect based on attack direction.
/// </summary>
public class SlashVFX : MonoBehaviour
{
    [Header("VFX Settings")]
    [SerializeField] private GameObject slashVFXPrefab;
    [SerializeField] private float vfxDuration = 0.5f;
    [SerializeField] private Vector3 vfxOffset = new Vector3(0.5f, 0f, 0f);
    [SerializeField] private float vfxScale = 1f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Spawn slash VFX at character's facing direction
    /// </summary>
    public void SpawnSlashEffect()
    {
        if (slashVFXPrefab == null)
        {
            Debug.LogError("Slash VFX Prefab is not assigned!");
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
        }

        // Scale the effect
        slashEffect.transform.localScale = new Vector3(vfxScale, vfxScale, 1f);

        // Set sorting order to appear above player
        if (slashSprite != null)
        {
            slashSprite.sortingOrder = 10;
        }

        // Destroy after duration
        Destroy(slashEffect, vfxDuration);

        Debug.Log($"Spawned slash VFX at {spawnPos}, facing direction: {(facingX > 0 ? "RIGHT" : "LEFT")}");
    }

    /// <summary>
    /// Spawn slash VFX with custom position offset
    /// </summary>
    public void SpawnSlashEffectAtOffset(Vector3 customOffset)
    {
        if (slashVFXPrefab == null)
        {
            Debug.LogError("Slash VFX Prefab is not assigned!");
            return;
        }

        float facingX = animator.GetFloat("X");
        Vector3 playerPos = transform.position;
        Vector3 offsetDirection = new Vector3(customOffset.x * facingX, customOffset.y, customOffset.z);
        Vector3 spawnPos = playerPos + offsetDirection;

        GameObject slashEffect = Instantiate(slashVFXPrefab, spawnPos, Quaternion.identity);

        SpriteRenderer slashSprite = slashEffect.GetComponent<SpriteRenderer>();
        if (slashSprite != null)
        {
            slashSprite.flipX = facingX < 0;
            slashSprite.sortingOrder = 10;
        }

        slashEffect.transform.localScale = new Vector3(vfxScale, vfxScale, 1f);
        Destroy(slashEffect, vfxDuration);
    }
}
