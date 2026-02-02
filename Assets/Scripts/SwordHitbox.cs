
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SwordHitbox : MonoBehaviour
{
    [Header("Debug Visualization")]
    [SerializeField] private bool showDebugVisual = true;
    [SerializeField] private Color debugColor = new Color(1f, 0f, 0f, 0.5f);

    [Header("Critical Hit Settings")]
    [SerializeField] [Range(0f, 100f)] private float criticalChance = 15f; // Percentage chance for critical hit
    [SerializeField] [Range(1f, 10f)] private float criticalMultiplier = 2f; // Damage multiplier for critical hits

    private int damage = 10;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private bool hasHitEnemy = false;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public bool HasHitEnemy()
    {
        return hasHitEnemy;
    }

    private void Start()
    {
        if (showDebugVisual)
        {
            CreateDebugVisual();
        }
    }

    private void CreateDebugVisual()
    {
        GameObject visualObj = new GameObject("DebugVisual");
        visualObj.transform.SetParent(transform, false);

        SpriteRenderer sr = visualObj.AddComponent<SpriteRenderer>();
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        sr.color = debugColor;
        sr.sortingOrder = 100;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        visualObj.transform.localScale = col.size;
        visualObj.transform.localPosition = col.offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[SwordHitbox {GetInstanceID()}] OnTriggerEnter2D with {other.gameObject.name}");

        if (hitEnemies.Contains(other.gameObject))
        {
            Debug.Log($"[SwordHitbox {GetInstanceID()}] Already hit {other.gameObject.name}, skipping");
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            hitEnemies.Add(other.gameObject);
            hasHitEnemy = true;

            // Calculate critical hit
            bool isCritical = Random.Range(0f, 100f) < criticalChance;
            int finalDamage = damage;

            if (isCritical)
            {
                finalDamage = Mathf.RoundToInt(damage * criticalMultiplier);
                Debug.Log($"[SwordHitbox {GetInstanceID()}] CRITICAL HIT on {other.gameObject.name} for {finalDamage} damage!");
            }
            else
            {
                Debug.Log($"[SwordHitbox {GetInstanceID()}] Hitting {other.gameObject.name} for {finalDamage} damage");
            }

            // Play appropriate sound (critical hit for crits, normal hit for regular hits)
            if (AudioManager.Instance != null)
            {
                if (isCritical)
                {
                    AudioManager.Instance.PlayCriticalHitSound();
                }
                else
                {
                    AudioManager.Instance.PlayHitSound();
                }
            }

            // Show damage text
            if (DamageTextManager.Instance != null)
            {
                DamageTextManager.Instance.ShowDamage(other.transform.position, finalDamage, isCritical);
            }

            // Apply damage
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(finalDamage);
            }
            else
            {
                Debug.LogWarning($"{other.gameObject.name} has Enemy tag but no IDamageable component!");
            }
        }
    }

    private void OnDestroy()
    {
        hitEnemies.Clear();
    }
}

public interface IDamageable
{
    void TakeDamage(int damage);
}
