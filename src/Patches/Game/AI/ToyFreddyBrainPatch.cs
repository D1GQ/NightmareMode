using HarmonyLib;
using NightmareMode.Managers;

namespace NightmareMode.Patches.Game.AI;

[HarmonyPatch(typeof(ToyFreddyBrain))]
internal class ToyFreddyBrainPatch
{
    [HarmonyPatch(nameof(ToyFreddyBrain.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix(ToyFreddyBrain __instance)
    {
        AIManager.Toy_FreddyAI = __instance;
        __instance.StartTimer = 100f;

        if (!NightmarePlugin.ModEnabled) return true;
        return false;
    }
}