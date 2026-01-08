using HarmonyLib;
using NightmareMode.Helpers;
using NightmareMode.Monos;
using UnityEngine;

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

    [HarmonyPatch(nameof(VentCharacter.Update))]
    [HarmonyPostfix]
    private static void Update_Postfix(VentCharacter __instance)
    {
        if (NightmarePlugin.ModEnabled)
        {
            if (__instance.active)
            {
                if (Utils.InCameras())
                {
                    __instance.entertimer -= Time.deltaTime * 4f;
                }
            }
        }
    }
}