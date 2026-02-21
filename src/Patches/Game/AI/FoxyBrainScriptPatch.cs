using HarmonyLib;
using NightmareMode.Managers;
using NightmareMode.Modules.AI;

namespace NightmareMode.Patches.Game.AI;

[HarmonyPatch(typeof(FoxyBrainScript))]
internal class FoxyBrainScriptPatch
{
    [HarmonyPatch(nameof(FoxyBrainScript.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix(FoxyBrainScript __instance)
    {
        AIManager.W_FoxyAI = new FoxyAIWrapper(__instance);

        if (!NightmarePlugin.ModEnabled) return true;
        return false;
    }

    [HarmonyPatch(nameof(FoxyBrainScript.Update))]
    [HarmonyPrefix]
    private static bool Update_Prefix(FoxyBrainScript __instance)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        if (NightManager.InPowerSurge)
        {
            if (__instance.StartTimer <= 0f && !__instance.Active)
            {
                return false;
            }

            if (__instance.flash9 && __instance.Active)
            {
                return false;
            }
        }

        return true;
    }
}