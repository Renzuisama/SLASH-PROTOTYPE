using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float defaultDuration = 0.5f;
    [SerializeField] private float defaultMagnitude = 0.3f;

    private Vector3 originalPosition;
    private bool isShaking = false;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// Shake the camera with default settings
    /// </summary>
    public void Shake()
    {
        Shake(defaultDuration, defaultMagnitude);
    }

    /// <summary>
    /// Shake the camera with custom duration and magnitude
    /// </summary>
    /// <param name="duration">How long the shake lasts in seconds</param>
    /// <param name="magnitude">How intense the shake is (recommended: 0.1 - 0.5)</param>
    public void Shake(float duration, float magnitude)
    {
        if (isShaking)
        {
            // If already shaking, restart with new values
            StopAllCoroutines();
        }

        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        isShaking = true;
        originalPosition = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Generate random offset
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Apply offset to camera
            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;

            // Gradually reduce magnitude over time for smoother ending
            float currentMagnitude = magnitude * (1f - (elapsed / duration));
            magnitude = currentMagnitude;

            yield return null;
        }

        // Return to original position
        transform.localPosition = originalPosition;
        isShaking = false;
    }

    /// <summary>
    /// Stop shaking immediately and return to original position
    /// </summary>
    public void StopShake()
    {
        StopAllCoroutines();
        transform.localPosition = originalPosition;
        isShaking = false;
    }

    /// <summary>
    /// Check if camera is currently shaking
    /// </summary>
    public bool IsShaking()
    {
        return isShaking;
    }

    // Preset shake types for convenience
    public void ShakeLight()
    {
        Shake(0.3f, 0.1f);
    }

    public void ShakeMedium()
    {
        Shake(0.5f, 0.2f);
    }

    public void ShakeHeavy()
    {
        Shake(0.7f, 0.4f);
    }

    public void ShakeExplosion()
    {
        Shake(0.8f, 0.5f);
    }
}
