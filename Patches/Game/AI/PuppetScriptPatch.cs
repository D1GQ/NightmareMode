using HarmonyLib;
using NightmareMode.Managers;

namespace NightmareMode.Patches.Game.AI;

[HarmonyPatch(typeof(PuppetScript))]
internal class PuppetScriptPatch
{
    [HarmonyPatch(nameof(PuppetScript.Start))]
    [HarmonyPrefix]
    private static void Start_Prefix(PuppetScript __instance)
    {
        AIManager.PuppetAI = __instance;
    }
}