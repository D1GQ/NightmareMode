using HarmonyLib;

namespace NightmareMode.Patches.Game;

[HarmonyPatch(typeof(NewCameraMovement))]
internal class NewCameraMovementPatch
{
    [HarmonyPatch(nameof(NewCameraMovement.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix(NewCameraMovement __instance)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        // set faster movement
        __instance.Maxspeed = 100;
        return false;
    }
}