using UnityEngine;

/// <summary>
/// Animates the Holy Cross effect by cycling through sprite frames
/// </summary>
public class HolyCrossAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Sprite[] animationFrames;
    [SerializeField] private float frameRate = 12f; // Frames per second
    [SerializeField] private bool loop = false;
    [SerializeField] private bool playOnStart = true;

    [Header("Visual Settings")]
    [SerializeField] private float scale = 2f;
    [SerializeField] private Color tintColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private bool isPlaying = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Set initial scale
        transform.localScale = new Vector3(scale, scale, scale);

        // Set color tint
        spriteRenderer.color = tintColor;

        // Set sorting order
        spriteRenderer.sortingLayerName = "Default";
        spriteRenderer.sortingOrder = 100;
    }

    private void Start()
    {
        if (playOnStart && animationFrames != null && animationFrames.Length > 0)
        {
            Play();
        }
    }

    private void Update()
    {
        if (!isPlaying || animationFrames == null || animationFrames.Length == 0)
        {
            return;
        }

        frameTimer += Time.deltaTime;

        if (frameTimer >= 1f / frameRate)
        {
            frameTimer = 0f;
            currentFrame++;

            if (currentFrame >= animationFrames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    isPlaying = false;
                    currentFrame = animationFrames.Length - 1;
                    return;
                }
            }

            spriteRenderer.sprite = animationFrames[currentFrame];
        }
    }

    public void Play()
    {
        currentFrame = 0;
        frameTimer = 0f;
        isPlaying = true;

        if (animationFrames != null && animationFrames.Length > 0)
        {
            spriteRenderer.sprite = animationFrames[0];
        }
    }

    public void Stop()
    {
        isPlaying = false;
    }

    public void SetFrames(Sprite[] frames)
    {
        animationFrames = frames;
    }

    public void SetScale(float newScale)
    {
        scale = newScale;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetColor(Color color)
    {
        tintColor = color;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}
