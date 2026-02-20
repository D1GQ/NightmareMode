using NightmareMode.Records;
using System.Reflection;
using UnityEngine;

namespace NightmareMode.Modules;

internal static class Translator
{
    private const string LANG_PATH = "NightmareMode.Resources.Lang";
    private const SystemLanguage FALLBACK_LANGUAGE = SystemLanguage.English;
    private static Dictionary<SystemLanguage, LanguageData> _translations = [];

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
                    keysTranslated = [.. keyValuePairs.Keys]
                };
                _translations[language] = languageData;
                NightmarePlugin.Log.LogInfo($"Loaded translations for {language} ({fileName})");
            }
        }
    }

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

    private static void ParseJsonToDictionary(string jsonContent, Dictionary<string, string> keyValuePairs)
    {
        jsonContent = jsonContent.Trim();

        if (jsonContent.StartsWith("{") && jsonContent.EndsWith("}"))
        {
            jsonContent = jsonContent.Substring(1, jsonContent.Length - 2);
        }

        string[] pairs = jsonContent.Split(',');

        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split(':');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim().Trim('"');
                string value = keyValue[1].Trim().Trim('"');
                keyValuePairs[key] = value;
            }
        }
    }

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

    internal static string GetLanguageAndRegionCode(SystemLanguage systemLanguage)
    {
        return systemLanguage switch
        {
            SystemLanguage.Afrikaans => "af_ZA",
            SystemLanguage.Arabic => "ar_SA",
            SystemLanguage.Basque => "eu_ES",
            SystemLanguage.Belarusian => "be_BY",
            SystemLanguage.Bulgarian => "bg_BG",
            SystemLanguage.Catalan => "ca_ES",
            SystemLanguage.Chinese => "zh_CN",
            SystemLanguage.Czech => "cs_CZ",
            SystemLanguage.Danish => "da_DK",
            SystemLanguage.Dutch => "nl_NL",
            SystemLanguage.English => "en_US",
            SystemLanguage.Estonian => "et_EE",
            SystemLanguage.Faroese => "fo_FO",
            SystemLanguage.Finnish => "fi_FI",
            SystemLanguage.French => "fr_FR",
            SystemLanguage.German => "de_DE",
            SystemLanguage.Greek => "el_GR",
            SystemLanguage.Hebrew => "he_IL",
            SystemLanguage.Hungarian => "hu_HU",
            SystemLanguage.Icelandic => "is_IS",
            SystemLanguage.Indonesian => "id_ID",
            SystemLanguage.Italian => "it_IT",
            SystemLanguage.Japanese => "ja_JP",
            SystemLanguage.Korean => "ko_KR",
            SystemLanguage.Latvian => "lv_LV",
            SystemLanguage.Lithuanian => "lt_LT",
            SystemLanguage.Norwegian => "no_NO",
            SystemLanguage.Polish => "pl_PL",
            SystemLanguage.Portuguese => "pt_PT",
            SystemLanguage.Romanian => "ro_RO",
            SystemLanguage.Russian => "ru_RU",
            SystemLanguage.SerboCroatian => "sh_RS",
            SystemLanguage.Slovak => "sk_SK",
            SystemLanguage.Slovenian => "sl_SI",
            SystemLanguage.Spanish => "es_ES",
            SystemLanguage.Swedish => "sv_SE",
            SystemLanguage.Thai => "th_TH",
            SystemLanguage.Turkish => "tr_TR",
            SystemLanguage.Ukrainian => "uk_UA",
            SystemLanguage.Vietnamese => "vi_VN",
            SystemLanguage.ChineseSimplified => "zh_CN",
            SystemLanguage.ChineseTraditional => "zh_TW",
            SystemLanguage.Unknown => "unknown",
            _ => $"{systemLanguage.ToString().ToLower()}_XX"
        };
    }
}