using UnityEngine;

/// <summary>
/// Animates a slash effect using sprite frames.
/// Attach this to your SlashEffect prefab.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SlashAnimator : MonoBehaviour
{
    [Header("Animation Frames")]
    [Tooltip("Drag all your slash sprite frames here in order")]
    [SerializeField] private Sprite[] frames;

    [Header("Animation Settings")]
    [SerializeField] private float frameRate = 24f; // Frames per second
    [SerializeField] private bool loop = false;
    [SerializeField] private bool destroyOnComplete = true;

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private bool isPlaying = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If no frames assigned, try to use the current sprite
        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("SlashAnimator: No frames assigned! Assign sprite frames in the Inspector.");
        }
        else
        {
            // Start with first frame
            spriteRenderer.sprite = frames[0];
        }
    }

    private void Update()
    {
        if (!isPlaying || frames == null || frames.Length == 0) return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= 1f / frameRate)
        {
            frameTimer = 0f;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    isPlaying = false;
                    if (destroyOnComplete)
                    {
                        Destroy(gameObject);
                    }
                    return;
                }
            }

            spriteRenderer.sprite = frames[currentFrame];
        }
    }

    /// <summary>
    /// Play the animation from the beginning
    /// </summary>
    public void Play()
    {
        currentFrame = 0;
        frameTimer = 0f;
        isPlaying = true;
        if (frames != null && frames.Length > 0)
        {
            spriteRenderer.sprite = frames[0];
        }
    }

    /// <summary>
    /// Stop the animation
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
    }
}
