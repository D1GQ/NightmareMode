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

    // Bug fix
    [HarmonyPatch(nameof(VentCharacter.DeActivateVent))]
    [HarmonyPrefix]
    private static bool DeActivateVent_Prefix(VentCharacter __instance)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        if (__instance.killed)
        {
            OfficeLightScript.dead = true;
            __instance.camanim.SetTrigger(__instance.CamScare);
            __instance.Scare.SetActive(true);
            __instance.lightmnger.maskval -= 10f;
            __instance.self.SetActive(false);
            return false;
        }

        return true;
    }
}