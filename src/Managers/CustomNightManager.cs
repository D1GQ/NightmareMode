using NightmareMode.Helpers;
using NightmareMode.Items.Enums;
using NightmareMode.Modules;
using NightmareMode.Monos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareMode.Managers;

internal static class CustomNightManager
{
    internal static readonly Dictionary<AITypes, int> AILevels = [];

    internal static void LoadMenu(NightUI nightUI)
    {
        _spawnedCharacters.Clear();
        InitLevels();
        if (nightUI != null)
        {
            int max = NightmarePlugin.CustomNightMaxAILevelAll?.Value ?? 20;
            int maxPuppet = NightmarePlugin.CustomNightMaxAILevelPuppet?.Value ?? 10;
            CreateCharacter(nightUI, AITypes.FreddyAI, Translator.Get("Character.ToyFreddy"), 0, max, LoadPortraitFromSheet((1, 1)));
            CreateCharacter(nightUI, AITypes.BonnieAI, Translator.Get("Character.ToyBonnie"), 0, max, LoadPortraitFromSheet((2, 1)));
            CreateCharacter(nightUI, AITypes.ChicaAI, Translator.Get("Character.ToyChica"), 0, max, LoadPortraitFromSheet((3, 1)));
            CreateCharacter(nightUI, AITypes.MangleAI, Translator.Get("Character.Mangle"), 0, max, LoadPortraitFromSheet((4, 1)));
            CreateCharacter(nightUI, AITypes.BBAI, Translator.Get("Character.BB"), 0, max, LoadPortraitFromSheet((5, 1)));
            CreateCharacter(nightUI, AITypes.PuppetAI, Translator.Get("Character.Puppet"), 0, maxPuppet, LoadPortraitFromSheet((1, 2)));
            CreateCharacter(nightUI, AITypes.WFreddyAI, Translator.Get("Character.Freddy"), 0, max, LoadPortraitFromSheet((2, 2)));
            CreateCharacter(nightUI, AITypes.WBonnieAI, Translator.Get("Character.Bonnie"), 0, max, LoadPortraitFromSheet((3, 2)));
            CreateCharacter(nightUI, AITypes.WChicaAI, Translator.Get("Character.Chica"), 0, max, LoadPortraitFromSheet((4, 2)));
            CreateCharacter(nightUI, AITypes.FOXYAI, Translator.Get("Character.Foxy"), 0, max, LoadPortraitFromSheet((5, 2)));
        }
    }


    private static Sprite? LoadPortraitFromSheet((int col, int row) target) => Utils.LoadSpriteFromSheet($"NightmareMode.Resources.Images.portraits.png", 5, 2, target);

    private static void InitLevels()
    {
        var types = Enum.GetValues(typeof(AITypes));
        foreach (AITypes ai in types)
        {
            if (!AILevels.ContainsKey(ai))
            {
                AILevels[ai] = 0;
            }
        }
    }

    private static GameObject? charactersObj;
    private static void CreateCharacter(NightUI nightUI, AITypes ai, string name, int minAI, int maxAi, Sprite? portrait = null)
    {
        if (charactersObj == null)
        {
            charactersObj = new GameObject("Characters");
            charactersObj.transform.SetParent(nightUI.transform, false);
        }

        var characterAI = CreatePrefab(charactersObj.transform);
        if (characterAI != null)
        {
            characterAI.SetButtonAction(() =>
            {
                int plus = 1;
                if (Input.GetKey(KeyCode.LeftShift))
                    plus = 5;
                AILevels[ai] += plus;
                var level = AILevels[ai] = Mathf.Clamp(AILevels[ai], minAI, maxAi);
                characterAI.SetAILevelText(level.ToString());
                characterAI.UpdateButtonState(minAI, maxAi, level);
            });
            characterAI.SetButtonAction(() =>
            {
                int plus = 1;
                if (Input.GetKey(KeyCode.LeftShift))
                    plus = 5;
                AILevels[ai] -= plus;
                var level = AILevels[ai] = Mathf.Clamp(AILevels[ai], minAI, maxAi);
                characterAI.SetAILevelText(level.ToString());
                characterAI.UpdateButtonState(minAI, maxAi, level);
            }, false);
            characterAI.Setup(name, ai, portrait);
            characterAI.SetAILevelText(AILevels[ai].ToString());
            characterAI.UpdateButtonState(minAI, maxAi, AILevels[ai]);
            SortUI(characterAI);
        }
    }

    private static readonly List<CustomNightAIUI> _spawnedCharacters = new List<CustomNightAIUI>();
    private static void SortUI(CustomNightAIUI characterAI)
    {
        _spawnedCharacters.Add(characterAI);

        int maxPerRow = 5;
        float spacingX = 5f;
        float spacingY = 8f;
        Vector3 offset = new(0f, 2.5f, 0f);

        for (int i = 0; i < _spawnedCharacters.Count; i++)
        {
            int row = i < maxPerRow ? 0 : 1;
            int col = i % maxPerRow;

            float posX = (col - 2f) * spacingX;
            float posY = (row == 0 ? 1 : -1) * spacingY;

            _spawnedCharacters[i].transform.localPosition = new Vector3(
                posX + offset.x,
                posY + offset.y,
                0f + offset.z
            );
        }
    }

    private static CustomNightAIUI CreatePrefab(Transform parent)
    {
        var prefab = new GameObject("CustomNightAI(Prefab)").AddComponent<CustomNightAIUI>();
        prefab.transform.SetParent(parent, false);
        prefab.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        var night = Utils.FindInactive("Canvas/NightsPage/Night1");
        if (night != null)
        {
            var Forward = night.transform.Find("Forward")?.GetComponent<Button>();
            var Back = night.transform.Find("Back")?.GetComponent<Button>();
            var RawImage = night.transform.Find("RawImage")?.GetComponent<RawImage>();
            var RawImageOutline = night.transform.Find("RawImage (1)")?.GetComponent<RawImage>();
            var TextTMP = night.transform.Find("Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>();
            prefab.SetPrefab(RawImage, RawImageOutline, Forward, Back, TextTMP);
        }

        return prefab;
    }
}
