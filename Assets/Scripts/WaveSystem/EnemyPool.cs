using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple object pool for enemy GameObjects
/// </summary>
public class EnemyPool : MonoBehaviour
{
    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, GameObject> activeObjects = new Dictionary<GameObject, GameObject>();
    private Transform poolContainer;

    private void Awake()
    {
        poolContainer = new GameObject("PoolContainer").transform;
        poolContainer.SetParent(transform);
    }

    /// <summary>
    /// Get an enemy from the pool or create new one
    /// </summary>
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("EnemyPool: Cannot spawn null prefab");
            return null;
        }

        // Ensure pool exists for this prefab
        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new Queue<GameObject>();
        }

        GameObject obj;

        if (pools[prefab].Count > 0)
        {
            // Reuse from pool
            obj = pools[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            // Reset poolable components
            IPoolable[] poolables = obj.GetComponents<IPoolable>();
            foreach (var poolable in poolables)
            {
                poolable.OnSpawnFromPool();
            }
        }
        else
        {
            // Create new
            obj = Instantiate(prefab, position, rotation);
            obj.transform.SetParent(poolContainer);
        }

        activeObjects[obj] = prefab;
        return obj;
    }

    /// <summary>
    /// Return an enemy to the pool
    /// </summary>
    public void Return(GameObject obj)
    {
        if (obj == null) return;

        if (!activeObjects.ContainsKey(obj))
        {
            Debug.LogWarning($"EnemyPool: Trying to return object {obj.name} that wasn't spawned from pool");
            Destroy(obj);
            return;
        }

        GameObject prefab = activeObjects[obj];
        activeObjects.Remove(obj);

        // Call poolable cleanup
        IPoolable[] poolables = obj.GetComponents<IPoolable>();
        foreach (var poolable in poolables)
        {
            poolable.OnReturnToPool();
        }

        obj.SetActive(false);
        obj.transform.SetParent(poolContainer);

        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new Queue<GameObject>();
        }

        pools[prefab].Enqueue(obj);
    }

    /// <summary>
    /// Clear all pools
    /// </summary>
    public void ClearAll()
    {
        foreach (var pool in pools.Values)
        {
            while (pool.Count > 0)
            {
                Destroy(pool.Dequeue());
            }
        }

        pools.Clear();

        foreach (var obj in activeObjects.Keys)
        {
            if (obj != null)
                Destroy(obj);
        }

        activeObjects.Clear();
    }

    /// <summary>
    /// Get count of active enemies spawned from a prefab
    /// </summary>
    public int GetActiveCount(GameObject prefab)
    {
        int count = 0;
        foreach (var kvp in activeObjects)
        {
            if (kvp.Value == prefab)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Get total active enemy count
    /// </summary>
    public int GetTotalActiveCount()
    {
        return activeObjects.Count;
    }
}
