using HarmonyLib;
using NightmareMode.Enums;
using NightmareMode.Managers;
using NightmareMode.Modules.AI;

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
                    AIManager.Toy_BonnieAI = new AnimatronicAIWrapper(__instance);
                }
                break;
            case nameof(AITypes.ChicaAI):
                {
                    AIManager.Toy_ChicaAI = new AnimatronicAIWrapper(__instance);
                }
                break;
            case nameof(AITypes.MangleAI):
                {
                    AIManager.MangleAI = new AnimatronicAIWrapper(__instance);
                }
                break;
            case nameof(AITypes.WFreddyAI):
                {
                    AIManager.W_FreddyAI = new AnimatronicAIWrapper(__instance);
                }
                break;
            case nameof(AITypes.WBonnieAI):
                {
                    AIManager.W_BonnieAI = new AnimatronicAIWrapper(__instance);
                }
                break;
            case nameof(AITypes.WChicaAI):
                {
                    AIManager.W_ChicaAI = new AnimatronicAIWrapper(__instance);
                }
                break;
        }

        if (!NightmarePlugin.ModEnabled) return true;

        return false;
    }
}