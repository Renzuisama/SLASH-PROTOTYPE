using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float moveDistance = 1f;

    [Header("Critical Hit Settings")]
    [SerializeField] private float criticalScaleMultiplier = 1.5f;
    [SerializeField] private float criticalMoveDistance = 1.5f;

    private TextMeshPro textMesh;
    private Color startColor;
    private Vector3 startPosition;
    private float elapsedTime = 0f;
    private bool isCritical = false;
    private float actualMoveDistance;

    private void Awake()
    {
        // Try to find TextMeshPro on this GameObject first
        textMesh = GetComponent<TextMeshPro>();

        // If not found, search in children
        if (textMesh == null)
        {
            textMesh = GetComponentInChildren<TextMeshPro>();
        }

        if (textMesh != null)
        {
            startColor = textMesh.color;
        }
        else
        {
            Debug.LogError("DamageText: No TextMeshPro component found! Make sure the prefab has a TextMeshPro component.");
        }
    }

    private void Start()
    {
        startPosition = transform.position;
        actualMoveDistance = isCritical ? criticalMoveDistance : moveDistance;
    }

    private void Update()
    {
        if (textMesh == null) return;

        elapsedTime += Time.deltaTime;

        // Move upward
        float moveAmount = (elapsedTime / fadeDuration) * actualMoveDistance;
        transform.position = startPosition + Vector3.up * moveAmount;

        // Fade out
        float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        // Destroy when fade completes
        if (elapsedTime >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int damage)
    {
        if (textMesh != null)
        {
            textMesh.text = damage.ToString();
        }
    }

    public void SetDamage(int damage, bool critical)
    {
        isCritical = critical;

        if (textMesh != null)
        {
            textMesh.text = damage.ToString();

            if (critical)
            {
                // Make text larger for critical hits
                textMesh.fontSize *= criticalScaleMultiplier;
            }
        }
    }

    public void SetColor(Color color)
    {
        if (textMesh != null)
        {
            startColor = color;
            textMesh.color = color;
        }
    }
}
