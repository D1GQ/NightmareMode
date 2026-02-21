using NightmareMode.Enums;
using NightmareMode.Helpers;
using NightmareMode.Modules;
using NightmareMode.Monos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareMode.Managers;

/// <summary>
/// Manages the Custom Night mode UI and AI level configuration.
/// Handles the creation and layout of character selection UI elements,
/// stores AI difficulty levels, and manages the custom night setup process.
/// </summary>
internal static class CustomNightManager
{
    /// <summary>
    /// Dictionary storing the current AI difficulty level for each animatronic type.
    /// </summary>
    internal static readonly Dictionary<AITypes, int> AILevels = [];

    /// <summary>
    /// Loads the Custom Night menu UI, creating character selection elements
    /// for all available animatronics with their appropriate portraits and AI level controls.
    /// </summary>
    /// <param name="nightUI">The NightUI component that will host the custom night interface.</param>
    internal static void LoadCustomNightMenu(NightUI nightUI)
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

    /// <summary>
    /// Loads a character portrait sprite from the portrait sprite sheet.
    /// </summary>
    /// <param name="target">A tuple containing the column and row (1-based) of the desired portrait in the sprite sheet.</param>
    /// <returns>The loaded sprite, or null if loading failed.</returns>
    private static Sprite? LoadPortraitFromSheet((int col, int row) target) => Utils.LoadSpriteFromSheet($"NightmareMode.Resources.Images.portraits.png", 5, 2, target);

    /// <summary>
    /// Initializes all AI types with a default level of 0.
    /// Ensures every animatronic type has an entry in the AILevels dictionary.
    /// </summary>
    private static void InitLevels()
    {
        var types = Enum.GetValues(typeof(AITypes));
        foreach (AITypes ai in types)
        {
            if (ai == AITypes.None) continue;

            if (!AILevels.ContainsKey(ai))
            {
                AILevels[ai] = 0;
            }
        }
    }

    private static GameObject? charactersObj;

    /// <summary>
    /// Creates a UI element for a single animatronic in the custom night menu.
    /// Sets up the character's portrait, name, AI level display, and control buttons.
    /// </summary>
    /// <param name="nightUI">The parent NightUI component.</param>
    /// <param name="ai">The animatronic type.</param>
    /// <param name="name">The display name for the character.</param>
    /// <param name="minAI">The minimum allowed AI level (typically 0).</param>
    /// <param name="maxAi">The maximum allowed AI level.</param>
    /// <param name="portrait">Optional sprite for the character portrait.</param>
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
            // Setup plus button action (increase AI level)
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

            // Setup minus button action (decrease AI level)
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

    /// <summary>
    /// Arranges the character UI elements in a grid layout.
    /// Positions characters in two rows with five columns, centered on the screen.
    /// </summary>
    /// <param name="characterAI">The character UI element to add to the layout.</param>
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

    /// <summary>
    /// Creates and configures a prefab instance for a character UI element.
    /// Sets up the visual components by cloning existing UI elements from the night selection screen.
    /// </summary>
    /// <param name="parent">The transform to parent the new UI element under.</param>
    /// <returns>The configured CustomNightAIUI component, or null if creation failed.</returns>
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