#pragma warning disable CS8618

using UnityEngine;

namespace NightmareMode.Records;

internal record LanguageData
{
    internal SystemLanguage languageType;
    internal Dictionary<string, string> keyValuePairs = [];
    internal bool HasTranslation(string key) => keyValuePairs.ContainsKey(key);
}
