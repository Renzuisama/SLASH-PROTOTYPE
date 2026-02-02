using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip swingSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip criticalHitSound; // Special sound for critical hits
    [SerializeField] private AudioClip dashSound; // Dash/dodge sound effect
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip healthPickupSound;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    private float lastHitSoundTime = 0f;
    private float lastCritSoundTime = 0f;
    private float lastSwingSoundTime = 0f;
    private const float soundCooldown = 0.1f; // Minimum time between hit sounds
    private const float swingSoundCooldown = 0.15f; // Minimum time between swing sounds

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create audio source if not assigned
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySwingSound()
    {
        // Prevent sound stacking with cooldown
        if (Time.time - lastSwingSoundTime < swingSoundCooldown)
        {
            return;
        }

        if (swingSound != null)
        {
            sfxSource.PlayOneShot(swingSound);
            lastSwingSoundTime = Time.time;
            Debug.Log("Playing swing sound");
        }
        else
        {
            Debug.LogWarning("Swing sound is not assigned!");
        }
    }

    public void PlayHitSound()
    {
        // Prevent sound stacking with cooldown
        if (Time.time - lastHitSoundTime < soundCooldown)
        {
            return;
        }

        if (hitSound != null)
        {
            // Stop swing sound if it's playing
            if (sfxSource.isPlaying && sfxSource.clip == swingSound)
            {
                sfxSource.Stop();
            }
            sfxSource.PlayOneShot(hitSound);
            lastHitSoundTime = Time.time;
            Debug.Log("Playing hit sound");
        }
        else
        {
            Debug.LogWarning("Hit sound is not assigned!");
        }
    }

    public void PlayCriticalHitSound()
    {
        // Prevent sound stacking with cooldown
        if (Time.time - lastCritSoundTime < soundCooldown)
        {
            return;
        }

        if (criticalHitSound != null)
        {
            // Stop swing sound if it's playing
            if (sfxSource.isPlaying && sfxSource.clip == swingSound)
            {
                sfxSource.Stop();
            }
            sfxSource.PlayOneShot(criticalHitSound);
            lastCritSoundTime = Time.time;
            Debug.Log("Playing CRITICAL HIT sound!");
        }
        else if (hitSound != null)
        {
            // Fallback to normal hit if critical hit sound not assigned
            if (sfxSource.isPlaying && sfxSource.clip == swingSound)
            {
                sfxSource.Stop();
            }
            sfxSource.PlayOneShot(hitSound);
            lastCritSoundTime = Time.time;
            Debug.LogWarning("Critical hit sound is not assigned! Using normal hit sound.");
        }
        else
        {
            Debug.LogWarning("Neither hit sound nor critical hit sound is assigned!");
        }
    }

    public void PlayDashSound()
    {
        if (dashSound != null)
        {
            sfxSource.PlayOneShot(dashSound);
            Debug.Log("Playing dash sound");
        }
        else
        {
            Debug.LogWarning("Dash sound is not assigned!");
        }
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            sfxSource.PlayOneShot(deathSound);
            Debug.Log("Playing death sound");
        }
        else
        {
            Debug.LogWarning("Death sound is not assigned!");
        }
    }

    public void PlayHealthPickupSound()
    {
        if (healthPickupSound != null)
        {
            sfxSource.PlayOneShot(healthPickupSound);
            Debug.Log("Playing health pickup sound");
        }
        else
        {
            Debug.LogWarning("Health pickup sound is not assigned!");
        }
    }
}
