using HarmonyLib;
using NightmareMode.Managers;

namespace NightmareMode.Patches.Game.AI;

[HarmonyPatch(typeof(FoxyBrainScript))]
internal class FoxyBrainScriptPatch
{
    [HarmonyPatch(nameof(FoxyBrainScript.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix(FoxyBrainScript __instance)
    {
        AIManager.W_FoxyAI = __instance;

        if (!NightmarePlugin.ModEnabled) return true;
        return false;
    }
}