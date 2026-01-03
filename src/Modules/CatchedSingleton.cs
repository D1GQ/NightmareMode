using UnityEngine;

namespace NightmareMode.Modules;

internal static class CatchedSingleton<T> where T : MonoBehaviour
{
    private static T? _cachedInstance;

    internal static T Instance => GetSingleton();

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