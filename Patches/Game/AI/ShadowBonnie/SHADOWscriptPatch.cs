using HarmonyLib;

namespace NightmareMode.Patches.Game.AI.ShadowBonnie;

[HarmonyPatch(typeof(SHADOWscript))]
internal class SHADOWscriptPatch
{
    // Remove Mini Shadow Bonnies in cameras entirely in Nightmare Mode
    [HarmonyPatch(nameof(SHADOWscript.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix(SHADOWscript __instance)
    {
        if (NightmarePlugin.ModEnabled)
        {
            UnityEngine.Object.Destroy(__instance.gameObject);
            return false;
        }

        return true;
    }
}
