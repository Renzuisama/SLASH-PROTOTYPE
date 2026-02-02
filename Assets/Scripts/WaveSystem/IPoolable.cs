/// <summary>
/// Interface for objects that can be pooled
/// Implement this on components that need to reset state when reused
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Called when object is taken from pool and reactivated
    /// </summary>
    void OnSpawnFromPool();

    /// <summary>
    /// Called when object is returned to pool and deactivated
    /// </summary>
    void OnReturnToPool();
}
