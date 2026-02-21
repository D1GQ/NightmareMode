#pragma warning disable CS8602

using NightmareMode.Enums;
using NightmareMode.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareMode.Monos;

/// <summary>
/// Manages the UI elements for a single character in the Custom Night mode.
/// Handles the display of character portraits, AI level controls, and button interactions
/// for adjusting AI difficulty levels.
/// </summary>
internal sealed class CustomNightAIUI : MonoBehaviour
{
    private bool hasSet;
    private AITypes AI;
    private List<Component?> components = [];
    private TextMeshProUGUI? NameTMP;
    private TextMeshProUGUI? AILevelTMP;
    private RawImage? CharacterImage;
    private List<Button?> AIButtons = [];
    private Action OnAIPlusButton = () => { };
    private Action OnAIMinusButton = () => { };

    /// <summary>
    /// Sets up the visual prefabs and initializes the UI components for the character.
    /// Creates and positions the character portrait, name text, AI level text, and control buttons.
    /// </summary>
    /// <param name="characterImage">The raw image component template for the character portrait.</param>
    /// <param name="characterImageOutline">The raw image component template for the portrait outline.</param>
    /// <param name="aiPlusButton">The button template for increasing AI difficulty.</param>
    /// <param name="aiMinusButton">The button template for decreasing AI difficulty.</param>
    /// <param name="textTMP">The text template for displaying character name and AI level.</param>
    internal void SetPrefab(RawImage? characterImage, RawImage? characterImageOutline, Button? aiPlusButton, Button? aiMinusButton, TextMeshProUGUI? textTMP)
    {
        var characterPortrait = new GameObject("Portrait");
        characterPortrait.transform.SetParent(transform, false);
        characterImageOutline = Instantiate(characterImageOutline, characterPortrait.transform);
        characterImageOutline.name = "Outline";
        characterImage = Instantiate(characterImage, characterPortrait.transform);
        characterImage.name = "Image";
        characterImage.color = Color.white;
        characterImage.gameObject.SetLocalSpace(Vector3.zero, new Vector3(0.05f, 0.1f, 1f));
        characterImageOutline.gameObject.SetLocalSpace(Vector3.zero, new Vector3(0.053f, 0.105f, 1f));
        CharacterImage = characterImage;

        textTMP = Instantiate(textTMP, transform);
        textTMP.gameObject.transform.localPosition = new Vector3(0f, 8f, 0f);
        textTMP.color = Color.white;
        textTMP.fontSize = 7;
        textTMP.name = "Name";
        textTMP.raycastTarget = false;
        NameTMP = textTMP;
        AILevelTMP = Instantiate(textTMP, transform);
        AILevelTMP.name = "AILevel";
        AILevelTMP.gameObject.transform.localPosition = new Vector3(0f, -6.5f, 0f);

        var buttons = new GameObject("Buttons");
        buttons.transform.SetParent(transform, false);
        aiPlusButton = Instantiate(aiPlusButton, buttons.transform);
        aiPlusButton.GetComponent<RectTransform>().sizeDelta = new(28f, 25f);
        aiPlusButton.name = "Plus";
        aiMinusButton = Instantiate(aiMinusButton, buttons.transform);
        aiMinusButton.GetComponent<RectTransform>().sizeDelta = new(28f, 25f);
        aiMinusButton.name = "Minus";
        aiPlusButton.gameObject.SetLocalSpace(new Vector3(2.2f, -7.5f), new Vector3(0.06f, 0.13f, 1f));
        aiMinusButton.gameObject.SetLocalSpace(new Vector3(-2.2f, -7.5f), new Vector3(0.06f, 0.13f, 1f));
        aiPlusButton.onClick = new();
        aiMinusButton.onClick = new();
        AIButtons.Add(aiPlusButton);
        AIButtons.Add(aiMinusButton);

        components.Add(characterImage);
        components.Add(characterImageOutline);
        components.Add(aiPlusButton);
        components.Add(aiMinusButton);
        components.Add(textTMP);

        NameTMP.SetText("Character");
        SetAILevelText("0");
    }

    /// <summary>
    /// Configures the UI element with specific character data.
    /// Sets the character name, AI type, and portrait image.
    /// </summary>
    /// <param name="name">The display name of the character.</param>
    /// <param name="ai">The AI type enum value for this character.</param>
    /// <param name="characterPortrait">Optional sprite containing the character's portrait.</param>
    internal void Setup(string name, AITypes ai, Sprite? characterPortrait = null)
    {
        if (components.Any(c => c == null)) return;

        if (hasSet) return;
        hasSet = true;
        NameTMP.SetText(name);
        AI = ai;
        LoadImage(characterPortrait);
        SetupButtons();
        gameObject.name = $"CustomNightAI({Enum.GetName(typeof(AITypes), AI)})";
    }

    /// <summary>
    /// Loads and crops the character portrait sprite to display in the UI.
    /// Extracts the sprite region from the source texture and applies it to the RawImage.
    /// </summary>
    /// <param name="characterPortrait">The sprite containing the character portrait.</param>
    private void LoadImage(Sprite? characterPortrait)
    {
        if (characterPortrait != null)
        {
            Rect spriteRect = characterPortrait.rect;

            Texture2D originalTexture = characterPortrait.texture;
            Texture2D croppedTexture = new(
                (int)spriteRect.width,
                (int)spriteRect.height
            );

            Color[] pixels = originalTexture.GetPixels(
                (int)spriteRect.x,
                originalTexture.height - (int)spriteRect.y - (int)spriteRect.height,
                (int)spriteRect.width,
                (int)spriteRect.height
            );

            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();

            CharacterImage.texture = croppedTexture;
        }
        else
        {
            CharacterImage.texture = null;
        }
    }

    /// <summary>
    /// Initializes the plus and minus buttons with their click handlers.
    /// Sets up the button click events to trigger the appropriate actions.
    /// </summary>
    private void SetupButtons()
    {
        if (AIButtons.Count != 2) return;

        AIButtons[0].onClick = new();
        AIButtons[0].onClick.AddListener(OnAIPlusButton.Invoke);
        AIButtons[1].onClick = new();
        AIButtons[1].onClick.AddListener(OnAIMinusButton.Invoke);
    }

    /// <summary>
    /// Enables or disables a specific AI control button and updates its visual state.
    /// </summary>
    /// <param name="active">True to enable the button, false to disable it.</param>
    /// <param name="isPlusButton">True for the plus button, false for the minus button.</param>
    private void SetButtonActive(bool active, bool isPlusButton = true)
    {
        int b = isPlusButton ? 0 : 1;
        var button = AIButtons[b];
        button.enabled = active;
        button.GetComponentsInChildren<TextMeshProUGUI>().Last().color = active ? Color.white : new Color(0.1f, 0.1f, 0.1f);
    }

    /// <summary>
    /// Sets the action to be performed when an AI control button is clicked.
    /// </summary>
    /// <param name="action">The action to execute on button click.</param>
    /// <param name="isPlusButton">True for the plus button, false for the minus button.</param>
    internal void SetButtonAction(Action action, bool isPlusButton = true)
    {
        if (isPlusButton)
        {
            OnAIPlusButton = action;
        }
        else
        {
            OnAIMinusButton = action;
        }
    }

    /// <summary>
    /// Updates the enabled state of the plus and minus buttons based on current AI level limits.
    /// Disables buttons when the AI level has reached the minimum or maximum allowed value.
    /// </summary>
    /// <param name="min">The minimum allowed AI level.</param>
    /// <param name="max">The maximum allowed AI level.</param>
    /// <param name="cur">The current AI level.</param>
    internal void UpdateButtonState(int min, int max, int cur)
    {
        SetButtonActive(false);
        SetButtonActive(false, false);
        if (cur < max)
        {
            SetButtonActive(true);
        }
        if (cur > min)
        {
            SetButtonActive(true, false);
        }
    }

    /// <summary>
    /// Updates the displayed AI level text.
    /// </summary>
    /// <param name="str">The string to display as the AI level (typically a number in parentheses).</param>
    internal void SetAILevelText(string str) => AILevelTMP?.SetText($"({str})");
}