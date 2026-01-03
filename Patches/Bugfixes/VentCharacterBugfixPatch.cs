using HarmonyLib;

namespace NightmareMode.Patches.Bugfixes;

[HarmonyPatch(typeof(VentCharacter))]
internal class VentCharacterBugfixPatch
{
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