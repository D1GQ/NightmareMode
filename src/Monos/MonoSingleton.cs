#pragma warning disable CS8603

using UnityEngine;

namespace NightmareMode.Monos;

/// <summary>
/// A generic base class for creating singleton MonoBehaviour components.
/// Ensures only one instance of the derived class exists at any time and provides
/// global access to that instance.
/// </summary>
/// <typeparam name="T">The type of the singleton class that derives from MonoSingleton&lt;T&gt;.</typeparam>
internal abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static (T? _null, bool _exists) _wasCreated = (null, false);

    private static T? _instance;

    /// <summary>
    /// Gets the singleton instance of type T.
    /// If the instance doesn't exist, it will be created automatically.
    /// </summary>
    internal static T Instance
    {
        get
        {
            if (_wasCreated._exists && _instance == null)
            {
                Create();
            }
            return _instance;
        }
        private set => _instance = value;
    }

    /// <summary>
    /// Creates the singleton instance if it doesn't already exist.
    /// </summary>
    /// <param name="dontDestroy">If true, the GameObject will persist across scene loads using DontDestroyOnLoad.</param>
    internal static void Create(bool dontDestroy = true)
    {
        if (_instance != null)
        {
            NightmarePlugin.Log.LogWarning($"{typeof(T).Name} already exists.");
            return;
        }

        var obj = new GameObject($"Singleton({typeof(T).Name})");
        _instance = obj.AddComponent<T>();
        _wasCreated = (null, true);
        NightmarePlugin.Log.LogInfo($"Created singleton {typeof(T).Name}");

        if (dontDestroy)
        {
            DontDestroyOnLoad(obj);
        }
    }

    /// <summary>
    /// Unity Awake method that ensures singleton integrity.
    /// Handles instance registration and duplicate destruction.
    /// </summary>
    protected virtual void Awake()
    {
        if (this is T instance)
        {
            if (!_wasCreated._exists)
            {
                if (_instance != null)
                {
                    Destroy(_instance);
                    _instance = null;
                }

                _instance = instance;
            }
            else if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
        }
        else
        {
            Destroy(this);
            return;
        }

        _Awake();
    }

    /// <summary>
    /// Unity OnDestroy method that cleans up the instance reference when the singleton is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _OnDestroy();
        }
    }

    /// <summary>
    /// Virtual method that can be overridden by derived classes to add custom awake logic.
    /// Called after singleton instance validation is complete.
    /// </summary>
    protected virtual void _Awake() { }

    /// <summary>
    /// Virtual method that can be overridden by derived classes to add custom destruction logic.
    /// Called when the singleton instance is being destroyed.
    /// </summary>
    protected virtual void _OnDestroy() { }
}