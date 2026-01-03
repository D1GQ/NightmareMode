using NightmareMode.Items.Enums;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace NightmareMode.Data;

internal class DataManager
{
    internal static List<string> PlayerPrefs => _settingsData.playerPrefs;
    private static readonly bool _doCompress = true;
    private static SettingsData _settingsData = new();
    internal static string _settingsFilePath = Path.Combine(NightmarePlugin.GetGamePath(), $"NightmareMode.dat");

    private static string Compress(string data)
    {
        if (!_doCompress) return data;
        byte[] flattenedData = Encoding.UTF8.GetBytes(data);
        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
        {
            gzip.Write(flattenedData, 0, flattenedData.Length);
        }
        ms.Position = 0;
        return Convert.ToBase64String(ms.ToArray());
    }

    private static string Decompress(string data)
    {
        if (!_doCompress) return data;
        byte[] compressedBytes = Convert.FromBase64String(data.Trim());
        using var ms = new MemoryStream(compressedBytes);
        using var gzip = new GZipStream(ms, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip);
        return reader.ReadToEnd();
    }

    internal static void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                string data = File.ReadAllText(_settingsFilePath);
                if (string.IsNullOrEmpty(data))
                {
                    SaveSettings();
                    return;
                }
                string json = Decompress(data);
                _settingsData = JsonUtility.FromJson<SettingsData>(json) ?? new SettingsData();
            }
            SaveSettings();
        }
        catch (Exception ex)
        {
            NightmarePlugin.Log.LogError(ex);
            SaveSettings();
        }
    }

    internal static void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(_settingsData);
            string compressedJson = Compress(json);
            File.WriteAllText(_settingsFilePath, compressedJson);
        }
        catch (Exception ex)
        {
            NightmarePlugin.Log.LogError(ex);
        }
    }

    internal static void TrySavePrefs()
    {
        string combined = string.Join("", _settingsData.playerPrefs);
        var hash = combined.GetHashCode();
        if (_settingsData.playerPrefsHash != hash)
        {
            _settingsData.playerPrefsHash = hash;
            SaveSettings();
        }
    }

    internal static void SetPref(string key, object value)
    {
        string typeName = value.GetType().Name;
        _settingsData.playerPrefs.RemoveAll(x => x.StartsWith(key + "/") && x.EndsWith(typeName));

        if (value == null)
        {
            return;
        }

        string valueString;

        switch (value)
        {
            case float f:
                valueString = f.ToString(CultureInfo.InvariantCulture);
                break;
            case double d:
                valueString = d.ToString(CultureInfo.InvariantCulture);
                break;
            case decimal dec:
                valueString = dec.ToString(CultureInfo.InvariantCulture);
                break;
            case Vector2 vec2:
                valueString = JsonUtility.ToJson(vec2);
                break;
            case Vector3 vec3:
                valueString = JsonUtility.ToJson(vec3);
                break;
            case Color color:
                valueString = JsonUtility.ToJson(color);
                break;
            default:
                valueString = value.ToString().Replace("/", "\\/");
                break;
        }

        _settingsData.playerPrefs.Add($"{key}/{valueString}/{typeName}");

        TrySavePrefs();
    }

    internal static T? GetPref<T>(string key)
    {
        try
        {
            string entry = _settingsData.playerPrefs
                .LastOrDefault(x => x.StartsWith(key + "/"));

            if (entry == null) return default;

            string[] parts = entry.Split('/');
            if (parts.Length < 3) return default;

            string valueString = parts[1].Replace("\\/", "/");
            string typeName = parts[2];

            if (typeName == "null" || valueString == "null")
                return default;

            object value = typeName switch
            {
                "Byte" => byte.Parse(valueString),
                "SByte" => sbyte.Parse(valueString),
                "Int16" => short.Parse(valueString),
                "UInt16" => ushort.Parse(valueString),
                "Int32" => int.Parse(valueString),
                "UInt32" => uint.Parse(valueString),
                "Int64" => long.Parse(valueString),
                "UInt64" => ulong.Parse(valueString),
                "Single" => float.Parse(valueString, CultureInfo.InvariantCulture),
                "Double" => double.Parse(valueString, CultureInfo.InvariantCulture),
                "Decimal" => decimal.Parse(valueString, CultureInfo.InvariantCulture),
                "Boolean" => bool.Parse(valueString),
                "Char" => char.Parse(valueString),
                "String" => valueString,
                "Vector2" => JsonUtility.FromJson<Vector2>(valueString),
                "Vector3" => JsonUtility.FromJson<Vector3>(valueString),
                "Color" => JsonUtility.FromJson<Color>(valueString),
                _ => throw new NotSupportedException($"Type {typeName} not supported")
            };

            Type underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            return (T)Convert.ChangeType(value, underlyingType);
        }
        catch (Exception ex)
        {
            NightmarePlugin.Log.LogError($"Failed to get pref {key}: {ex}");
            return default;
        }
    }

    [Serializable]
    private class SettingsData
    {
        public int playerPrefsHash = 0;
        public List<string> playerPrefs = [];

        public NightsFlag e_nightsCompleted = NightsFlag.None;
        public ChallengesFlag e_challengesCompleted = ChallengesFlag.None;
        public int i_nights = 1;
    }

    internal static class CompletedNights
    {
        internal static bool HasCompletedAll(NightsFlag nights) => (_settingsData.e_nightsCompleted & nights) == nights;
        internal static bool HasCompletedAny(NightsFlag nights) => (_settingsData.e_nightsCompleted & nights) != 0;
        internal static bool HasCompletedNight(NightsFlag night) => (_settingsData.e_nightsCompleted & night) == night;
        internal static void SetNightCompleted(NightsFlag night)
        {
            _settingsData.e_nightsCompleted |= night;
            SaveSettings();
        }
        internal static void UnsetNightCompleted(NightsFlag night)
        {
            _settingsData.e_nightsCompleted &= ~night;
            SaveSettings();
        }
    }

    internal static class CompletedChallenges
    {
        internal static bool HasCompletedAll(ChallengesFlag challenges) => (_settingsData.e_challengesCompleted & challenges) == challenges;
        internal static bool HasCompletedAny(ChallengesFlag challenges) => (_settingsData.e_challengesCompleted & challenges) != 0;
        internal static bool HasCompletedChallenge(ChallengesFlag challenge) => (_settingsData.e_challengesCompleted & challenge) == challenge;
        internal static void SetChallengeCompleted(ChallengesFlag challenge)
        {
            _settingsData.e_challengesCompleted |= challenge;
            SaveSettings();
        }
        internal static void UnsetChallengeCompleted(ChallengesFlag challenge)
        {
            _settingsData.e_challengesCompleted &= ~challenge;
            SaveSettings();
        }
    }

    internal static int Nights
    {
        get
        {
            return _settingsData.i_nights;
        }
        set
        {
            _settingsData.i_nights = value;
            SaveSettings();
        }
    }
}