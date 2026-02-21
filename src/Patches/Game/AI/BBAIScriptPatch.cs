using HarmonyLib;
using NightmareMode.Managers;
using NightmareMode.Modules.AI;

namespace NightmareMode.Patches.Game.AI;

[HarmonyPatch(typeof(BBAIScript))]
internal class BBAIScriptPatch
{
    [HarmonyPatch(nameof(BBAIScript.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix(BBAIScript __instance)
    {
        AIManager.BalloonBoyAI = new BBAIWrapper(__instance);

        if (!NightmarePlugin.ModEnabled) return true;
        return false;
    }
}