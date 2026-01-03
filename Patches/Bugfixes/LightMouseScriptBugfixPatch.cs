using HarmonyLib;
using UnityEngine;

namespace NightmareMode.Patches.Bugfixes;

[HarmonyPatch(typeof(LightMouseScript))]
internal class LightMouseScriptBugfixPatch
{
    // Bug fix
    [HarmonyPatch(nameof(LightMouseScript.Update))]
    [HarmonyPrefix]
    private static bool Update_Prefix(LightMouseScript __instance)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        if (OfficeLightScript.mask || GameObject.Find("Alive/GAMPLAYCOMPONENTS/Cameras")?.activeInHierarchy == true)
        {
            if (__instance.on)
            {
                __instance.light.SetActive(false);
                __instance.on = false;
                __instance.offsound.Play();
            }
        }

        return false;
    }
}