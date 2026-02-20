using NightmareMode.Modules;
using TMPro;
using UnityEngine;

namespace NightmareMode.Monos;

internal sealed class GUIAutoTranslator : MonoBehaviour
{
    private bool _setup;
    private string _originalText = string.Empty;
    private event Action<string>? setText;

    internal void Setup(TextMeshProUGUI? textMeshProUGUI)
    {
        if (textMeshProUGUI == null) return;
        if (_setup) return;
        _setup = true;

        _originalText = textMeshProUGUI.text;
        setText = (text) =>
        {
            textMeshProUGUI.SetText(text);
        };
        textMeshProUGUI.font = GetFont(textMeshProUGUI.font);

        UpdateText();
    }

    internal void Setup(TextMeshPro? textMeshPro)
    {
        if (textMeshPro == null) return;
        if (_setup) return;
        _setup = true;

        _originalText = textMeshPro.text;
        setText = (text) =>
        {
            textMeshPro.SetText(text);
        };
        textMeshPro.font = GetFont(textMeshPro.font);

        UpdateText();
    }

    private void UpdateText()
    {
        string key = GetKey();
        if (key == string.Empty) return;

        string translation = Translator.Get(key);
        setText?.Invoke(translation);
    }

    private TMP_FontAsset GetFont(TMP_FontAsset defaultFont)
    {
        return Application.systemLanguage switch
        {
            SystemLanguage.Korean => GetFontByName("NanumBrushScript-Regular SDF"),
            _ => defaultFont,
        };
    }

    private TMP_FontAsset GetFontByName(string fontName)
    {
        return Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
            .FirstOrDefault(f => f.name == fontName);
    }

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
            "Audio" => "Text.Audio",
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
