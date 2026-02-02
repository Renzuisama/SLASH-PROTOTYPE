using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private float hitboxDistance = 1f;
    [SerializeField] private int damage = 10;

    private GameObject currentHitbox;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Check for left mouse button press
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ActivateHitbox();
        }
    }

    public void ActivateHitbox()
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

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement is NULL!");
            return;
        }

        // Get player facing direction
        float facingX = playerMovement.GetFacingX();
        Debug.Log($"Player facing direction: {facingX} ({(facingX > 0 ? "RIGHT" : "LEFT")})");

        // Get hitbox size to properly position it
        BoxCollider2D prefabCollider = hitboxPrefab.GetComponent<BoxCollider2D>();
        float hitboxHalfWidth = prefabCollider != null ? prefabCollider.size.x / 2f : 0.5f;

        // Calculate spawn position: player position + (hitbox half width + distance) in facing direction
        Vector3 playerPos = transform.position;
        float totalOffset = (hitboxHalfWidth + hitboxDistance) * facingX;
        Vector3 spawnPos = playerPos + new Vector3(totalOffset, 0f, 0f);

        Debug.Log($"Player position: {playerPos}");
        Debug.Log($"Hitbox half width: {hitboxHalfWidth}");
        Debug.Log($"Total offset: {totalOffset}");
        Debug.Log($"Spawn position: {spawnPos}");

        // Spawn hitbox
        currentHitbox = Instantiate(hitboxPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"Hitbox instantiated: {currentHitbox != null}");

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
    
    public void DeactivateHitbox()
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
}
