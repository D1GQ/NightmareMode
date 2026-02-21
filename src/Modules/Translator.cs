using NightmareMode.Records;
using System.Reflection;
using UnityEngine;

namespace NightmareMode.Modules;

/// <summary>
/// Provides localization and translation services for the game.
/// Loads JSON language files from embedded resources and provides text translations
/// based on the current system language with fallback support.
/// </summary>
internal static class Translator
{
    private const string LANG_PATH = "NightmareMode.Resources.Lang";
    private const SystemLanguage FALLBACK_LANGUAGE = SystemLanguage.English;
    private static Dictionary<SystemLanguage, LanguageData> _translations = [];

    /// <summary>
    /// Initializes the translation system by loading all available language files
    /// from embedded resources. Called during mod startup.
    /// </summary>
    internal static void Initialize()
    {
        SystemLanguage[] languages = (SystemLanguage[])Enum.GetValues(typeof(SystemLanguage));
        _translations = [];
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (var language in languages)
        {
            string fileName = GetLanguageAndRegionCode(language);
            string resourcePath = $"{LANG_PATH}.{fileName}.json";

            if (TryLoadJsonFromResources(assembly, resourcePath, out var keyValuePairs))
            {
                var languageData = new LanguageData
                {
                    languageType = language,
                    keyValuePairs = keyValuePairs,
                };
                _translations[language] = languageData;
                NightmarePlugin.Log.LogInfo($"Loaded translations for {language} ({fileName})");
            }
        }
    }

    /// <summary>
    /// Attempts to load and parse a JSON language file from embedded resources.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resources.</param>
    /// <param name="resourcePath">The full path to the resource within the assembly.</param>
    /// <param name="keyValuePairs">Output dictionary containing the parsed key-value pairs.</param>
    /// <returns>True if the file was successfully loaded and parsed, false otherwise.</returns>
    private static bool TryLoadJsonFromResources(Assembly assembly, string resourcePath, out Dictionary<string, string> keyValuePairs)
    {
        keyValuePairs = [];

        try
        {
            using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                string[] resourceNames = assembly.GetManifestResourceNames();
                string? foundResource = resourceNames.FirstOrDefault(r => r == resourcePath);

                if (foundResource == null)
                    return false;

                using Stream altStream = assembly.GetManifestResourceStream(foundResource);
                if (altStream == null)
                    return false;

                using StreamReader reader = new(altStream);
                string jsonContent = reader.ReadToEnd();
                ParseJsonToDictionary(jsonContent, keyValuePairs);
            }
            else
            {
                using StreamReader reader = new(stream);
                string jsonContent = reader.ReadToEnd();
                ParseJsonToDictionary(jsonContent, keyValuePairs);
            }

            return keyValuePairs.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Parses a JSON string into a dictionary of key-value pairs.
    /// Handles basic JSON structure with string keys and values, including escaped characters.
    /// </summary>
    /// <param name="jsonContent">The JSON string to parse.</param>
    /// <param name="keyValuePairs">The dictionary to populate with parsed key-value pairs.</param>
    private static void ParseJsonToDictionary(string jsonContent, Dictionary<string, string> keyValuePairs)
    {
        jsonContent = jsonContent.Trim();

        if (jsonContent.StartsWith("{") && jsonContent.EndsWith("}"))
        {
            jsonContent = jsonContent.Substring(1, jsonContent.Length - 2).Trim();
        }

        bool inString = false;
        bool escapeNext = false;
        string currentKey = "";
        string currentValue = "";
        string currentToken = "";
        bool parsingKey = true;

        for (int i = 0; i < jsonContent.Length; i++)
        {
            char c = jsonContent[i];

            if (escapeNext)
            {
                currentToken += c switch
                {
                    'n' => '\n',
                    't' => '\t',
                    'r' => '\r',
                    '\\' => '\\',
                    '"' => '"',
                    _ => (object)c,
                };
                escapeNext = false;
                continue;
            }

            if (c == '\\' && inString)
            {
                escapeNext = true;
                continue;
            }

            if (c == '"')
            {
                inString = !inString;
                continue;
            }

            if (!inString)
            {
                if (c == ':' && parsingKey)
                {
                    currentKey = currentToken.Trim().Trim('"');
                    currentToken = "";
                    parsingKey = false;
                    continue;
                }

                if (c == ',')
                {
                    currentValue = currentToken.Trim().Trim('"');
                    if (!string.IsNullOrEmpty(currentKey))
                    {
                        keyValuePairs[currentKey] = currentValue;
                    }
                    currentToken = "";
                    parsingKey = true;
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
            }

            currentToken += c;
        }

        if (!parsingKey && !string.IsNullOrEmpty(currentKey))
        {
            currentValue = currentToken.Trim().Trim('"');
            keyValuePairs[currentKey] = currentValue;
        }
    }

    /// <summary>
    /// Retrieves a translated string for the specified key using the current system language.
    /// Falls back to English if the translation is missing for the current language.
    /// </summary>
    /// <param name="key">The translation key to look up.</param>
    /// <returns>The translated string, or a placeholder in the format "&lt;key&gt;" if not found.</returns>
    internal static string Get(string key)
    {
        SystemLanguage currentLanguage = Application.systemLanguage;

        if (_translations.TryGetValue(currentLanguage, out var languageData) &&
            languageData.HasTranslation(key))
        {
            return languageData.keyValuePairs[key];
        }

        if (currentLanguage != FALLBACK_LANGUAGE &&
            _translations.TryGetValue(FALLBACK_LANGUAGE, out var fallbackData) &&
            fallbackData.HasTranslation(key))
        {
            NightmarePlugin.Log.LogWarning($"Missing translation '{key}' for {currentLanguage}, using fallback");
            return fallbackData.keyValuePairs[key];
        }

        return $"<{key}>";
    }

    /// <summary>
    /// Retrieves a translated string and formats it with the provided parameters.
    /// Uses string.Format to insert parameters into the translated template.
    /// </summary>
    /// <param name="key">The translation key to look up.</param>
    /// <param name="format">Optional format parameters to insert into the translated string.</param>
    /// <returns>The formatted translated string, or the unformatted translation if formatting fails.</returns>
    internal static string Get(string key, params string[] format)
    {
        string value = Get(key);

        if (format != null && format.Length > 0)
        {
            try
            {
                return string.Format(value, format);
            }
            catch (FormatException)
            {
                NightmarePlugin.Log.LogError($"Invalid format string for key '{key}': '{value}'");
                return value;
            }
        }

        return value;
    }

    /// <summary>
    /// Converts a Unity SystemLanguage enum value to its corresponding language-region code.
    /// Used for constructing file paths to language resources.
    /// </summary>
    /// <param name="systemLanguage">The system language to convert.</param>
    /// <returns>A string in the format "xx_XX" representing the language and region code.</returns>
    internal static string GetLanguageAndRegionCode(SystemLanguage systemLanguage)
    {
        return systemLanguage switch
        {
            // Latin-based languages
            SystemLanguage.English => "en_US",
            SystemLanguage.French => "fr_FR",
            SystemLanguage.German => "de_DE",
            SystemLanguage.Italian => "it_IT",
            SystemLanguage.Spanish => "es_ES",
            SystemLanguage.Portuguese => "pt_PT",
            SystemLanguage.Dutch => "nl_NL",
            SystemLanguage.Turkish => "tr_TR",
            SystemLanguage.Polish => "pl_PL",
            SystemLanguage.Czech => "cs_CZ",
            SystemLanguage.Hungarian => "hu_HU",

            // Korean
            SystemLanguage.Korean => "ko_KR",

            SystemLanguage.Afrikaans => "af_ZA",
            SystemLanguage.Arabic => "ar_SA",
            SystemLanguage.Basque => "eu_ES",
            SystemLanguage.Belarusian => "be_BY",
            SystemLanguage.Bulgarian => "bg_BG",
            SystemLanguage.Catalan => "ca_ES",
            SystemLanguage.Chinese => "zh_CN",
            SystemLanguage.Danish => "da_DK",
            SystemLanguage.Estonian => "et_EE",
            SystemLanguage.Faroese => "fo_FO",
            SystemLanguage.Finnish => "fi_FI",
            SystemLanguage.Greek => "el_GR",
            SystemLanguage.Hebrew => "he_IL",
            SystemLanguage.Icelandic => "is_IS",
            SystemLanguage.Indonesian => "id_ID",
            SystemLanguage.Japanese => "ja_JP",
            SystemLanguage.Latvian => "lv_LV",
            SystemLanguage.Lithuanian => "lt_LT",
            SystemLanguage.Norwegian => "no_NO",
            SystemLanguage.Romanian => "ro_RO",
            SystemLanguage.Russian => "ru_RU",
            SystemLanguage.SerboCroatian => "sh_RS",
            SystemLanguage.Slovak => "sk_SK",
            SystemLanguage.Slovenian => "sl_SI",
            SystemLanguage.Swedish => "sv_SE",
            SystemLanguage.Thai => "th_TH",
            SystemLanguage.Ukrainian => "uk_UA",
            SystemLanguage.Vietnamese => "vi_VN",
            SystemLanguage.ChineseSimplified => "zh_CN",
            SystemLanguage.ChineseTraditional => "zh_TW",
            SystemLanguage.Unknown => "unknown",

            _ => $"{systemLanguage.ToString().ToLower()}_XX"
        };
    }
}