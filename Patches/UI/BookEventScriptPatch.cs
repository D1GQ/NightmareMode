using HarmonyLib;

namespace NightmareMode.Patches.UI;

[HarmonyPatch(typeof(BookEventScript))]
internal class BookEventScriptPatch
{
    // Prevent base game entries being enable in the notebook when pausing
    [HarmonyPatch(nameof(BookEventScript.Update))]
    [HarmonyPrefix]
    private static bool Update_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        return false;
    }
}