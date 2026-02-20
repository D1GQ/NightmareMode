using HarmonyLib;
using NightmareMode.Monos;
using TMPro;

namespace NightmareMode.Patches.Unity;

[HarmonyPatch]
internal static class GUIAutoTranslatorPatch
{
    [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.Awake))]
    [HarmonyPostfix]
    private static void TextMeshProUGUI_Awake_Postfix(TextMeshProUGUI __instance)
    {
        var GUIAutoTranslator = __instance.gameObject.AddComponent<GUIAutoTranslator>();
        GUIAutoTranslator.Setup(__instance);
    }

    [HarmonyPatch(typeof(TextMeshPro), nameof(TextMeshPro.Awake))]
    [HarmonyPostfix]
    private static void TextMeshPro_Awake_Postfix(TextMeshPro __instance)
    {
        var GUIAutoTranslator = __instance.gameObject.AddComponent<GUIAutoTranslator>();
        GUIAutoTranslator.Setup(__instance);
    }
}
