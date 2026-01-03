using HarmonyLib;

namespace NightmareMode.Patches.Bugfixes;

[HarmonyPatch(typeof(LightManagerScript))]
internal class LightManagerScriptBugfixPatch
{
    // Bug fix
    [HarmonyPatch(nameof(LightManagerScript.Update))]
    [HarmonyPostfix]
    private static void Update_Postfix(LightManagerScript __instance)
    {
        if (!(__instance.bs1.Switches + __instance.bs2.Switches + __instance.bs3.Switches + __instance.bs4.Switches < 56))
        {
            __instance.PowerFreddy?.transform?.Find("Global Volume")?.gameObject?.SetActive(false);
        }
    }
}