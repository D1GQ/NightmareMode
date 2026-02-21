using UnityEngine;

namespace NightmareMode.Helpers;

/// <summary>
/// Provides extension methods for Unity GameObject to simplify common transform operations.
/// Helps reduce boilerplate code when setting local transform properties.
/// </summary>
internal static class GameObjectExtension
{
    /// <summary>
    /// Sets the local position, scale, and/or rotation of a GameObject in a single method call.
    /// Only modifies the properties that are explicitly provided; others remain unchanged.
    /// </summary>
    /// <param name="obj">The GameObject to modify. If null, the method returns without error.</param>
    /// <param name="pos">Optional local position to set. If null, position is not modified.</param>
    /// <param name="scale">Optional local scale to set. If null, scale is not modified.</param>
    /// <param name="rot">Optional local rotation to set. If null, rotation is not modified.</param>
    internal static void SetLocalSpace(this GameObject? obj, Vector3? pos = null, Vector3? scale = null, Quaternion? rot = null)
    {
        if (obj == null) return;
        if (pos is Vector3 POS)
            obj.transform.localPosition = POS;
        if (scale is Vector3 SCALE)
            obj.transform.localScale = SCALE;
        if (rot is Quaternion ROT)
            obj.transform.localRotation = ROT;
    }
}