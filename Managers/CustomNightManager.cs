using NightmareMode.Helpers;
using NightmareMode.Items.Enums;
using NightmareMode.Monos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareMode.Managers;

internal static class CustomNightManager
{
    internal static readonly Dictionary<AITypes, int> AILevels = [];

    internal static void LoadMenu(GameObject nightObj)
    {
        _spawnedCharacters.Clear();
        InitLevels();
        if (nightObj != null)
        {
            int max = NightmarePlugin.CustomNightMaxAILevelAll?.Value ?? 20;
            int maxPuppet = NightmarePlugin.CustomNightMaxAILevelPuppet?.Value ?? 10;
            CreateCharacter(nightObj, AITypes.FreddyAI, "Toy Freddy", 0, max, LoadPortraitFromSheet((1, 1)));
            CreateCharacter(nightObj, AITypes.BonnieAI, "Toy Bonnie", 0, max, LoadPortraitFromSheet((2, 1)));
            CreateCharacter(nightObj, AITypes.ChicaAI, "Toy Chica", 0, max, LoadPortraitFromSheet((3, 1)));
            CreateCharacter(nightObj, AITypes.MangleAI, "Mangle", 0, max, LoadPortraitFromSheet((4, 1)));
            CreateCharacter(nightObj, AITypes.BBAI, "BB", 0, max, LoadPortraitFromSheet((5, 1)));
            CreateCharacter(nightObj, AITypes.PuppetAI, "Puppet", 0, maxPuppet, LoadPortraitFromSheet((1, 2)));
            CreateCharacter(nightObj, AITypes.WFreddyAI, "Freddy", 0, max, LoadPortraitFromSheet((2, 2)));
            CreateCharacter(nightObj, AITypes.WBonnieAI, "Bonnie", 0, max, LoadPortraitFromSheet((3, 2)));
            CreateCharacter(nightObj, AITypes.WChicaAI, "Chica", 0, max, LoadPortraitFromSheet((4, 2)));
            CreateCharacter(nightObj, AITypes.FOXYAI, "Foxy", 0, max, LoadPortraitFromSheet((5, 2)));
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
    private static void CreateCharacter(GameObject nightObj, AITypes ai, string name, int minAI, int maxAi, Sprite? portrait = null)
    {
        if (charactersObj == null)
        {
            charactersObj = new GameObject("Characters");
            charactersObj.transform.SetParent(nightObj.transform, false);
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

    private static readonly List<CustomNightAI_UI> _spawnedCharacters = new List<CustomNightAI_UI>();
    private static void SortUI(CustomNightAI_UI characterAI)
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

    private static CustomNightAI_UI CreatePrefab(Transform parent)
    {
        var prefab = new GameObject("CustomNightAI(Prefab)").AddComponent<CustomNightAI_UI>();
        prefab.transform.SetParent(parent, false);
        prefab.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        var night = Utils.FindInactive("Canvas/Night1");
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
