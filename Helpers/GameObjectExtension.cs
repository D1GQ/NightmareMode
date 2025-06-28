using UnityEngine;

namespace NightmareMode.Helpers;

internal static class GameObjectExtension
{
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
