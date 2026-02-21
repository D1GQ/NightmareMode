using UnityEngine;

namespace NightmareMode.Modules;

/// <summary>
/// A utility class that provides cached access to Unity components using FindObjectOfType.
/// Caches the found instance to improve performance on subsequent accesses.
/// </summary>
/// <typeparam name="T">The type of MonoBehaviour component to find and cache.</typeparam>
internal static class CatchedSingleton<T> where T : MonoBehaviour
{
    private static T? _cachedInstance;

    /// <summary>
    /// Gets the singleton instance of type T.
    /// If the instance is not cached or has been destroyed, it will be found again using FindObjectOfType.
    /// </summary>
    internal static T Instance => GetSingleton();

    /// <summary>
    /// Retrieves the singleton instance, either from cache or by finding it in the scene.
    /// Validates that the cached instance still exists before returning it.
    /// </summary>
    /// <returns>The found instance of type T, or null if none exists in the scene.</returns>
    private static T GetSingleton()
    {
        if (_cachedInstance != null && _cachedInstance.gameObject != null)
        {
            return _cachedInstance;
        }

        _cachedInstance = UnityEngine.Object.FindObjectOfType<T>(true);
        return _cachedInstance;
    }
}