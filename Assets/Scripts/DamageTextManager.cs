using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [Header("Prefab")]
    [SerializeField] private GameObject damageTextPrefab;

    [Header("Settings")]
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private Color normalDamageColor = Color.white;
    [SerializeField] private Color criticalDamageColor = Color.red;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowDamage(Vector3 position, int damage, bool isCritical = false)
    {
        Color color = isCritical ? criticalDamageColor : normalDamageColor;
        ShowDamage(position, damage, isCritical, color);
    }

    public void ShowDamage(Vector3 position, int damage, bool isCritical, Color customColor)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogWarning("DamageTextManager: Damage text prefab not assigned!");
            return;
        }

        Vector3 spawnPosition = position + spawnOffset;
        GameObject textObj = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);

        DamageText damageText = textObj.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.SetDamage(damage, isCritical);
            damageText.SetColor(customColor);
        }
    }
}
