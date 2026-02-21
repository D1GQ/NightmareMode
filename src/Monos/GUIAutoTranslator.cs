using NightmareMode.Managers;
using NightmareMode.Modules;
using TMPro;
using UnityEngine;

namespace NightmareMode.Monos;

/// <summary>
/// Automatically translates UI text elements based on the current game language.
/// Handles both TextMeshProUGUI and TextMeshPro components.
/// </summary>
internal sealed class GUIAutoTranslator : MonoBehaviour
{
    private bool _setup;
    private string _originalText = string.Empty;
    private event Action<string>? setText;
    private event Action? setFont;

    /// <summary>
    /// Sets up the translator for a TextMeshProUGUI component.
    /// </summary>
    /// <param name="textMeshProUGUI">The TextMeshProUGUI component to translate.</param>
    internal void Setup(TextMeshProUGUI? textMeshProUGUI)
    {
        if (textMeshProUGUI == null) return;
        if (_setup) return;
        _setup = true;

        _originalText = textMeshProUGUI.text;
        setText = (text) =>
        {
            textMeshProUGUI.SetText(text);
            textMeshProUGUI.enableWordWrapping = false;
        };
        setFont = () =>
        {
            textMeshProUGUI.font = GetFont(textMeshProUGUI.font);
        };

        UpdateText();
    }

    /// <summary>
    /// Sets up the translator for a TextMeshPro component.
    /// </summary>
    /// <param name="textMeshPro">The TextMeshPro component to translate.</param>
    internal void Setup(TextMeshPro? textMeshPro)
    {
        if (textMeshPro == null) return;
        if (_setup) return;
        _setup = true;

        _originalText = textMeshPro.text;
        setText = (text) =>
        {
            textMeshPro.SetText(text);
            textMeshPro.enableWordWrapping = false;
        };
        setFont = () =>
        {
            textMeshPro.font = GetFont(textMeshPro.font);
        };

        UpdateText();
    }

    /// <summary>
    /// Updates the text with the translated version based on the current language.
    /// </summary>
    private void UpdateText()
    {
        string key = GetKey();
        if (key == string.Empty) return;

        string translation = Translator.Get(key);
        setText?.Invoke(translation);
        setFont?.Invoke();
    }

    /// <summary>
    /// Gets the appropriate font asset based on the system language.
    /// </summary>
    /// <param name="defaultFont">The default font to use if no language-specific font is found.</param>
    /// <returns>The language-specific font asset or the default font.</returns>
    internal static TMP_FontAsset GetFont(TMP_FontAsset defaultFont)
    {
        return Translator.CurrentLanguage switch
        {
            SystemLanguage.ChineseSimplified => ModManager.Instance.NotoSans_SC ?? defaultFont,
            SystemLanguage.Japanese => ModManager.Instance.NotoSans_JP ?? defaultFont,
            SystemLanguage.Korean => ModManager.Instance.NotoSans_KR ?? defaultFont,
            SystemLanguage.Russian => ModManager.Instance.NotoSans ?? defaultFont,
            _ => defaultFont,
        };
    }

    /// <summary>
    /// Gets the translation key for the original text.
    /// </summary>
    /// <returns>The translation key if found, otherwise an empty string.</returns>
    private string GetKey()
    {
        return _originalText switch
        {
            // Main menu
            "   Five Nights At Freddy's" => "Text.Title",
            "REWRITTEN" => !NightmarePlugin.ModEnabled ? "Text.SubTitle" : "Text.SubTitleMod",
            "PLAY" => "Text.Play",
            "QUIT" => "Text.Quit",
            "Settings" => "Text.Settings",
            "ENTER PICTURE CODE" => "Text.Enter_Picture_Code",
            "Activate Code" => "Text.Activate_Code",
            "LOCKED!" => "Text.Locked",

            // Graphics Settings menu
            "Graphics Settings" => "Text.Graphics_Settings",
            "Low" => "Text.Graphics_Low",
            "Medium" => "Text.Graphics_Medium",
            "High" => "Text.Graphics_High",
            "Ultra" => "Text.Graphics_Ultra",

            // Audio Settings menu
            "Audio Settings" => "Text.Audio_Settings",
            "MASTER VOLUME" => "Text.Master_Volume",
            "SFX VOLUME" => "Text.SFX_Volume",
            "AMBIENCE VOLUME" => "Text.Ambience_Volume",
            "JUMPSCARE VOLUME" => "Text.Jumpscare_Volume",
            "MINIGAME VOLUME" => "Text.Minigame_Volume",
            "PHONE GUY VOLUME" => "Text.Phoneguy_Volume",
            "CLOSE" => "Text.Close",

            // UI
            "ENTER" => "Text.Enter",
            "RETURN" => "Text.Return",
            "LOADING..." => "Text.Loading",
            "If something kills you, the game will give you a tip on how to avoid it happening again" => "Text.Loading.Tip1",
            "Balloon Boy will not react to the empty Freddy head, watch how he reacts to your flashlight instead" => "Text.Loading.Tip2",
            "The Mangle will leave your office after lowering its head twice" => "Text.Loading.Tip3",
            "Gen 2 Freddy will laugh if he is about to mess with a breaker box" => "Text.Loading.Tip4",
            "You cannot spam the camera flashlight, stop trying..." => "Text.Loading.Tip5",

            // Log Book
            "Notes:" => "Text.Notes",
            "WEEK 1" => "Text.Week1",
            "MONDAY" => "Text.Monday",
            "TUESDAY" => "Text.Tuesday",
            "WEDNESDAY" => "Text.Wednesday",
            "THURSDAY" => "Text.Thursday",
            "FRIDAY" => "Text.Friday",
            "SATURDAY" => "Text.Saturday",

            _ => string.Empty,
        };
    }
}