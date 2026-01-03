#pragma warning disable CS8603

using UnityEngine;

namespace NightmareMode.Modules;

internal abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static (T? _null, bool _exists) _wasCreated = (null, false);

    private static T? _instance;
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

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _OnDestroy();
        }
    }

    protected virtual void _Awake() { }

    protected virtual void _OnDestroy() { }
}
