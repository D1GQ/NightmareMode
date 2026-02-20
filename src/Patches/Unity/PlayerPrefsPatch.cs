using HarmonyLib;
using NightmareMode.Data;
using UnityEngine;

namespace NightmareMode.Patches.Unity;

[HarmonyPatch(typeof(PlayerPrefs))]
internal class PlayerPrefsPatch
{
    [HarmonyPatch(nameof(PlayerPrefs.SetInt))]
    [HarmonyPrefix]
    private static bool SetInt_Prefix(string key, int value)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.SetPref(key, value);
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.SetFloat))]
    [HarmonyPrefix]
    private static bool SetFloat_Prefix(string key, float value)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.SetPref(key, value);
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.SetString))]
    [HarmonyPrefix]
    private static bool SetString_Prefix(string key, string value)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.SetPref(key, value);
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.GetInt), [typeof(string), typeof(int)])]
    [HarmonyPrefix]
    private static bool GetInt_Prefix(string key, ref int defaultValue, ref int __result)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        var val = DataManager.GetPref<int?>(key);
        __result = val ?? defaultValue;
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.GetFloat), [typeof(string), typeof(float)])]
    [HarmonyPrefix]
    private static bool GetFloat_Prefix(string key, ref float defaultValue, ref float __result)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        var val = DataManager.GetPref<float?>(key);
        __result = val ?? defaultValue;
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.GetString), [typeof(string), typeof(string)])]
    [HarmonyPrefix]
    private static bool GetString_Prefix(string key, ref string defaultValue, ref string __result)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        __result = DataManager.GetPref<string>(key) ?? defaultValue;
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.HasKey))]
    [HarmonyPrefix]
    private static bool HasKey_Prefix(string key, ref bool __result)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        __result = DataManager.PlayerPrefs.Any(x => x.StartsWith(key + "/"));
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.DeleteKey))]
    [HarmonyPrefix]
    private static bool DeleteKey_Prefix(string key)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.PlayerPrefs.RemoveAll(x => x.StartsWith(key + "/"));
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.DeleteAll))]
    [HarmonyPrefix]
    private static bool DeleteAll_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.PlayerPrefs.Clear();
        return false;
    }

    [HarmonyPatch(nameof(PlayerPrefs.Save))]
    [HarmonyPrefix]
    private static bool Save_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        DataManager.TrySavePrefs();
        return false;
    }
}