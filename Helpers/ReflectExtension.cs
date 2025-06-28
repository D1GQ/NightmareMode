using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace NightmareMode.Helpers;

internal static class ReflectExtension
{
    private const BindingFlags NonPublicFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    // Caches for FieldInfo, PropertyInfo, MethodInfo, and Type lookups
    private static readonly ConcurrentDictionary<string, FieldInfo> _fieldCache = new();
    private static readonly ConcurrentDictionary<string, PropertyInfo> _propertyCache = new();
    private static readonly ConcurrentDictionary<string, MethodInfo> _methodCache = new();
    private static readonly ConcurrentDictionary<string, Type> _nestedTypeCache = new();

    // ==== Field Operations (Cached) ====
    internal static T GetField<T>(this object obj, string name)
    {
        if (obj == null) return default;

        string cacheKey = $"{obj.GetType().FullName}:{name}";
        if (!_fieldCache.TryGetValue(cacheKey, out var field))
        {
            field = obj.GetType().GetField(name, NonPublicFlags) ?? throw new MissingFieldException($"Field '{name}' not found");
            _fieldCache[cacheKey] = field;
        }
        return (T)field.GetValue(obj);
    }

    internal static void SetField<T>(this object obj, string name, T value)
    {
        if (obj == null) return;

        string cacheKey = $"{obj.GetType().FullName}:{name}";
        if (!_fieldCache.TryGetValue(cacheKey, out var field))
        {
            field = obj.GetType().GetField(name, NonPublicFlags) ?? throw new MissingFieldException($"Field '{name}' not found");
            _fieldCache[cacheKey] = field;
        }
        field.SetValue(obj, value);
    }

    // ==== Static Field Operations (Cached) ====
    internal static T GetStaticField<T>(this Type type, string name)
    {
        string cacheKey = $"{type.FullName}:static:{name}";
        if (!_fieldCache.TryGetValue(cacheKey, out var field))
        {
            field = type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new MissingFieldException($"Static field '{name}' not found in {type.Name}");
            _fieldCache[cacheKey] = field;
        }
        return (T)field.GetValue(null);
    }

    internal static void SetStaticField<T>(this Type type, string name, T value)
    {
        string cacheKey = $"{type.FullName}:static:{name}";
        if (!_fieldCache.TryGetValue(cacheKey, out var field))
        {
            field = type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new MissingFieldException($"Static field '{name}' not found in {type.Name}");
            _fieldCache[cacheKey] = field;
        }
        field.SetValue(null, value);
    }

    // ==== Property Operations (Cached) ====
    internal static T GetProperty<T>(this object obj, string name)
    {
        if (obj == null) return default;

        string cacheKey = $"{obj.GetType().FullName}:{name}";
        if (!_propertyCache.TryGetValue(cacheKey, out var prop))
        {
            prop = obj.GetType().GetProperty(name, NonPublicFlags)
                ?? throw new MissingMemberException($"Property '{name}' not found");
            _propertyCache[cacheKey] = prop;
        }
        return (T)prop.GetValue(obj);
    }

    internal static void SetProperty<T>(this object obj, string name, T value)
    {
        if (obj == null) return;

        string cacheKey = $"{obj.GetType().FullName}:{name}";
        if (!_propertyCache.TryGetValue(cacheKey, out var prop))
        {
            prop = obj.GetType().GetProperty(name, NonPublicFlags)
                ?? throw new MissingMemberException($"Property '{name}' not found");
            _propertyCache[cacheKey] = prop;
        }
        prop.SetValue(obj, value);
    }

    internal static IEnumerator GetCoroutine(this object obj, string name, params object[] parameters)
    {
        if (obj == null) return default;

        object coroutineResult = obj.InvokeMethod(name, parameters);
        return (IEnumerator)coroutineResult;
    }

    internal static IEnumerator GetStaticCoroutine(this Type type, string name, params object[] parameters)
    {
        object coroutineResult = type.InvokeStaticMethod(name, parameters);
        return (IEnumerator)coroutineResult;
    }

    // ==== Method Operations (Cached) ====
    internal static object InvokeMethod(this object obj, string name, params object[] parameters)
    {
        if (obj == null) return default;

        string paramTypes = string.Join(",", parameters.Select(p => p.GetType().Name));
        string cacheKey = $"{obj.GetType().FullName}:{name}({paramTypes})";

        if (!_methodCache.TryGetValue(cacheKey, out var method))
        {
            method = obj.GetType().GetMethod(name, NonPublicFlags, null, parameters.Select(p => p.GetType()).ToArray(), null)
                ?? throw new MissingMethodException($"Method '{name}' not found");
            _methodCache[cacheKey] = method;
        }
        return method.Invoke(obj, parameters);
    }

    internal static object InvokeStaticMethod(this Type type, string name, params object[] parameters)
    {
        string paramTypes = string.Join(",", parameters.Select(p => p.GetType().Name));
        string cacheKey = $"{type.FullName}:static:{name}({paramTypes})";

        if (!_methodCache.TryGetValue(cacheKey, out var method))
        {
            method = type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic, null, parameters.Select(p => p.GetType()).ToArray(), null)
                ?? throw new MissingMethodException($"Static method '{name}' not found in {type.Name}");
            _methodCache[cacheKey] = method;
        }
        return method.Invoke(null, parameters);
    }

    // ==== Nested Type Lookup (Cached) ====
    internal static Type GetNestedType(this Type type, string name)
    {
        string cacheKey = $"{type.FullName}:{name}";
        if (!_nestedTypeCache.TryGetValue(cacheKey, out var nestedType))
        {
            nestedType = type.GetNestedType(name, BindingFlags.NonPublic)
                ?? throw new TypeLoadException($"Nested type '{name}' not found in {type.Name}");
            _nestedTypeCache[cacheKey] = nestedType;
        }
        return nestedType;
    }
}