using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NightmareMode.Data;
using NightmareMode.Items.Attributes;
using NightmareMode.Managers;
using NightmareMode.Modules;
using NightmareMode.Monos;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NightmareMode;

[BepInProcess("FNAFRewritten87")]
[BepInPlugin(ModInfo.MOD_GUID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
internal class NightmarePlugin : BaseUnityPlugin
{
    internal static NightmarePlugin Instance { get; private set; }

    internal static bool ModEnabled { get; private set; } = ModInfo.DEBUG;
    private static Harmony? Harmony;
    internal static ManualLogSource Log => Instance._log;
    private ManualLogSource? _log;

    private void Awake()
    {
        LoadOptions();

        _log = Logger;
        Instance = this;
        Harmony = new(ModInfo.MOD_GUID);
        Harmony.PatchAll();

        Application.runInBackground = true;
        SceneManager.activeSceneChanged += OnSceneChanged;
        Translator.Initialize();
        DataManager.LoadSettings();
        InstanceAttribute.RegisterAll();
    }

    private bool _hasLateLoad;
    internal void LateLoad()
    {
        if (_hasLateLoad) return;
        _hasLateLoad = true;

        ModManager.Create();
        Debugger.Create();
    }

    internal static ConfigEntry<int>? CustomNightMaxAILevelAll { get; private set; }
    internal static ConfigEntry<int>? CustomNightMaxAILevelPuppet { get; private set; }
    private void LoadOptions()
    {
        CustomNightMaxAILevelAll = Config.Bind(new("NightmareMode", "CustomNightMaxAILevelAll"), 20, new("The AI level cap for Custom Night for all characters other than puppet."));
        CustomNightMaxAILevelPuppet = Config.Bind(new("NightmareMode", "CustomNightMaxAILevelPuppet"), 10, new("The AI level cap for Custom Night Puppet."));
    }

    private static void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "1983Location")
        {
            SceneManager.LoadScene("title");
        }
    }

    internal static void SwitchMode()
    {
        if (SceneManager.GetActiveScene().name == "title")
        {
            var loading = CatchedSingleton<LoadingTip>.Instance;
            loading.tips.ToList().ForEach(Destroy);
            loading.tips = [];
            loading?.gameObject.SetActive(true);
            GameObject.Find("cinematic").SetActive(false);
            Instance.StartCoroutine(CoSwitchMode());
        }
    }

    internal static IEnumerator CoSwitchMode()
    {
        yield return new WaitForSeconds(2f);
        ModEnabled = !ModEnabled;
        SceneManager.LoadScene("title");
    }

    internal static string GetDataPath() => Application.persistentDataPath;
    internal static string GetGamePath() => Path.GetDirectoryName(Application.dataPath) ?? Application.dataPath;
}
