using HarmonyLib;
using NightmareMode.Managers;

namespace NightmareMode.Patches.Game.AI.ShadowBonnie;

[HarmonyPatch(typeof(ShadowBonnieMainScript))]
internal class ShadowBonnieMainScriptPatch
{
    // Disable Shadow Bonnie in Nightmare Mode
    [HarmonyPatch(nameof(ShadowBonnieMainScript.Update))]
    [HarmonyPrefix]
    private static bool Update_Prefix()
    {
        if (NightmarePlugin.ModEnabled)
        {
            if (NightManager.CurrentNight?.Night == 6)
            {
                return true;
            }

            return false;
        }

        return true;
    }
}
