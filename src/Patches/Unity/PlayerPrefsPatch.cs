using HarmonyLib;
using NightmareMode.Data;
using UnityEngine;

namespace NightmareMode.Patches.Unity;

[HarmonyPatch(typeof(PlayerPrefs))]
internal class PlayerPrefsPatch
{
    [HarmonyPatch(nameof(PlayerPrefs.SetInt))]
    [HarmonyPrefix]
    private static bool SetInt(string key, int value)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.SetPref(key, value);
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.SetFloat))]
    [HarmonyPrefix]
    private static bool SetFloat(string key, float value)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.SetPref(key, value);
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.SetString))]
    [HarmonyPrefix]
    private static bool SetString(string key, string value)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.SetPref(key, value);
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.GetInt), (new[] { typeof(string), typeof(int) }))]
    [HarmonyPrefix]
    private static bool GetInt(string key, ref int defaultValue, ref int __result)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        var val = DataManager.GetPref<int?>(key);
        __result = val ?? defaultValue;
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.GetFloat), (new[] { typeof(string), typeof(float) }))]
    [HarmonyPrefix]
    private static bool GetFloat(string key, ref float defaultValue, ref float __result)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        var val = DataManager.GetPref<float?>(key);
        __result = val ?? defaultValue;
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.GetString), (new[] { typeof(string), typeof(string) }))]
    [HarmonyPrefix]
    private static bool GetString(string key, ref string defaultValue, ref string __result)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        __result = DataManager.GetPref<string>(key) ?? defaultValue;
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.HasKey))]
    [HarmonyPrefix]
    private static bool HasKey(string key, ref bool __result)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        __result = DataManager.PlayerPrefs.Any(x => x.StartsWith(key + "/"));
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.DeleteKey))]
    [HarmonyPrefix]
    private static bool DeleteKey(string key)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.PlayerPrefs.RemoveAll(x => x.StartsWith(key + "/"));
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.DeleteAll))]
    [HarmonyPrefix]
    private static bool DeleteAll()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.PlayerPrefs.Clear();
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.Save))]
    [HarmonyPrefix]
    private static bool Save()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.TrySavePrefs();
        return false;
    }
}