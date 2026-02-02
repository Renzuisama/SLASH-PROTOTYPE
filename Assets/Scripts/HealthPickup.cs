using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float lifetime = 30f; // Despawn after 30 seconds if not picked up

    [Header("Visual Feedback")]
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;

    private Vector3 startPosition;
    private float spawnTime;

    private void Start()
    {
        startPosition = transform.position;
        spawnTime = Time.time;
    }

    private void Update()
    {
        // Bob up and down animation
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Check lifetime and despawn
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if player picked it up
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Only heal if player is not at full health
                if (playerHealth.GetCurrentHealth() < playerHealth.GetMaxHealth())
                {
                    playerHealth.Heal(healAmount);

                    // Play health pickup sound
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayHealthPickupSound();
                    }
                    Debug.Log($"Player healed for {healAmount} HP");

                    // Destroy the pickup
                    Destroy(gameObject);
                }
            }
        }
    }
}
