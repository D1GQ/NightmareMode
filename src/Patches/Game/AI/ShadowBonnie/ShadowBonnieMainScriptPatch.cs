using HarmonyLib;

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
            return false;
        }

        return true;
    }
}
