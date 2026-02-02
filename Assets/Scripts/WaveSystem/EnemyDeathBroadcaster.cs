using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Lightweight component that broadcasts enemy death events
/// Add this to all enemy prefabs
/// </summary>
public class EnemyDeathBroadcaster : MonoBehaviour
{
    public static event Action<GameObject> OnEnemyDied;

    [Tooltip("Delay before returning to pool (must be >= death animation length)")]
    public float poolReturnDelay = 2f;

    private bool hasDied = false;

    /// <summary>
    /// Call this when the enemy dies (from health component or elsewhere)
    /// </summary>
    public void BroadcastDeath()
    {
        if (hasDied) return;

        hasDied = true;

        // Broadcast immediately for counting
        OnEnemyDied?.Invoke(gameObject);

        // Start delayed pool return to allow death animation to play
        StartCoroutine(DelayedPoolReturn());
    }

    private IEnumerator DelayedPoolReturn()
    {
        // Wait for death animation to complete
        yield return new WaitForSeconds(poolReturnDelay);

        // The SpawnService will handle returning to pool via the death event
        // This coroutine just ensures we don't return too early
    }

    private void OnDestroy()
    {
        // Only broadcast if being destroyed (not pooled)
        if (!hasDied && gameObject.activeInHierarchy)
        {
            BroadcastDeath();
        }
    }

    private void OnDisable()
    {
        // Reset for pooling
        hasDied = false;
    }

    private void OnEnable()
    {
        // Reset when spawned from pool
        hasDied = false;
    }
}
