using HarmonyLib;
using NightmareMode.Monos;

namespace NightmareMode.Patches.Game.AI;

[HarmonyPatch(typeof(OfficeCharacterScript))]
internal class OfficeCharacterScriptPatch
{
    [HarmonyPatch(nameof(OfficeCharacterScript.MaskCheckOffice))]
    [HarmonyPrefix]
    private static bool MaskCheckOffice_Prefix()
    {
        if (Debugger.Instance._godMode)
        {
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(OfficeCharacterScript.LeaveOffice))]
    [HarmonyPrefix]
    private static bool LeaveOfficee_Prefix(OfficeCharacterScript __instance)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        if (__instance.killed)
        {
            OfficeLightScript.dead = true;
            __instance.camanim.SetTrigger(__instance.CamAnimation);
            __instance.animself.SetTrigger("Scare");
            __instance.lightmnger.maskval -= 10f;
            __instance.dietimer = 200f;
            return false;
        }

        return true;
    }
}