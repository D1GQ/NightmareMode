using NightmareMode.Helpers;
using NightmareMode.Items.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS8602

namespace NightmareMode.Monos;

internal class CustomNightAI_UI : MonoBehaviour
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

    private void SetupButtons()
    {
        if (AIButtons.Count != 2) return;

        AIButtons[0].onClick = new();
        AIButtons[0].onClick.AddListener(OnAIPlusButton.Invoke);
        AIButtons[1].onClick = new();
        AIButtons[1].onClick.AddListener(OnAIMinusButton.Invoke);
    }

    private void SetButtonActive(bool active, bool isPlusButton = true)
    {
        int b = isPlusButton ? 0 : 1;
        var button = AIButtons[b];
        button.enabled = active;
        button.GetComponentsInChildren<TextMeshProUGUI>().Last().color = active ? Color.white : new Color(0.1f, 0.1f, 0.1f);
    }

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

    internal void SetAILevelText(string str) => AILevelTMP?.SetText($"({str})");
}
