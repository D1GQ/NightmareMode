using HarmonyLib;

namespace NightmareMode.Patches.Game.Night;

[HarmonyPatch(typeof(OfficeLightScript))]
internal class OfficeLightScriptPatch
{

    // Don't enable decorations
    [HarmonyPatch(nameof(OfficeLightScript.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        OfficeLightScript.mask = false;
        OfficeLightScript.dead = false;

        return false;
    }
}