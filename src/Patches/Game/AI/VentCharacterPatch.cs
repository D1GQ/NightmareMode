using HarmonyLib;
using NightmareMode.Monos;

namespace NightmareMode.Patches.Game.AI;

[HarmonyPatch(typeof(VentCharacter))]
internal class VentCharacterPatch
{
    [HarmonyPatch(nameof(VentCharacter.MaskCheckVent))]
    [HarmonyPrefix]
    private static bool MaskCheckVent_Prefix()
    {
        if (Debugger.Instance._godMode)
        {
            return false;
        }

        return true;
    }
}