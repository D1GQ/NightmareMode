using HarmonyLib;
using NightmareMode.Items.Enums;
using NightmareMode.Managers;

namespace NightmareMode.Patches.Game.AI;

[HarmonyPatch(typeof(AnimatronicAIScript))]
internal class AnimatronicAIScriptPatch
{
    [HarmonyPatch(nameof(AnimatronicAIScript.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix(AnimatronicAIScript __instance)
    {
        switch (__instance.name)
        {
            case nameof(AITypes.BonnieAI):
                {
                    AIManager.Toy_BonnieAI = __instance;
                }
                break;
            case nameof(AITypes.ChicaAI):
                {
                    AIManager.Toy_ChicaAI = __instance;
                }
                break;
            case nameof(AITypes.MangleAI):
                {
                    AIManager.MangleAI = __instance;
                }
                break;
            case nameof(AITypes.WFreddyAI):
                {
                    AIManager.W_FreddyAI = __instance;
                }
                break;
            case nameof(AITypes.WBonnieAI):
                {
                    AIManager.W_BonnieAI = __instance;
                }
                break;
            case nameof(AITypes.WChicaAI):
                {
                    AIManager.W_ChicaAI = __instance;
                }
                break;
        }

        if (!NightmarePlugin.ModEnabled) return true;
        return false;
    }
}