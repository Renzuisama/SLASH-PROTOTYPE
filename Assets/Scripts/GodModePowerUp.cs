using UnityEngine;

/// <summary>
/// Power-up pickup that activates God Mode when collected by the player.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class GodModePowerUp : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private AudioClip pickupSound;

    [Header("Visual")]
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.2f;
    [SerializeField] private float rotateSpeed = 0f; // Set to 0 for no rotation

    private Vector3 startPosition;
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();

        // Make sure collider is trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Update()
    {
        // Bob up and down
        if (bobHeight > 0)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        // Rotate (optional)
        if (rotateSpeed > 0)
        {
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            // Get the GodModeEffect component on the player
            GodModeEffect godMode = other.GetComponent<GodModeEffect>();

            if (godMode != null)
            {
                // Activate god mode
                godMode.ActivateGodMode();
                Debug.Log("God Mode Power-Up collected!");

                // Play sound
                if (pickupSound != null)
                {
                    // Play at position so sound continues after object is destroyed
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // Destroy the pickup
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogWarning("Player doesn't have GodModeEffect component! Add it to the Player.");
            }
        }
    }
}
