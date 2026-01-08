using HarmonyLib;
using NightmareMode.Helpers;

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

        if (OfficeLightScript.mask || Utils.InCameras())
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