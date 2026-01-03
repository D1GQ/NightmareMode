using HarmonyLib;
using NightmareMode.Helpers;
using NightmareMode.Managers;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareMode.Patches.UI;

[HarmonyPatch(typeof(MenuScript))]
internal class MenuScriptPatch
{
    [HarmonyPatch(nameof(MenuScript.Start))]
    [HarmonyPrefix]
    private static void Start_Prefix(MenuScript __instance)
    {
        NightmarePlugin.Instance.LateLoad();

        NightManager.Clear();

        // Set up button to switch between modes
        var play = Utils.FindInactive("Canvas/Page1/Play");
        if (play != null)
        {
            var mode = UnityEngine.Object.Instantiate(play, play.transform.parent);
            mode.name = "Mode";
            var textTMP = mode.GetComponentInChildren<TextMeshProUGUI>();
            if (textTMP != null)
            {
                textTMP.enableWordWrapping = false;
                textTMP.fontSize = 20f;
                textTMP.alignment = TextAlignmentOptions.Left;
                textTMP.SetText(NightmarePlugin.ModEnabled ? "<#B2B2B2>(Vanilla)</color>" : "<#CC0000>(Nightmare)</color>");
            }
            var button = mode.GetComponent<Button>();
            if (button != null)
            {
                button.onClick = new();
                button.onClick.AddListener(NightmarePlugin.SwitchMode);
            }
            __instance.StartCoroutine(CoSlideModeIn(mode));
        }

        if (!NightmarePlugin.ModEnabled) return;

        var curtain = GameObject.Find("1983curtain");
        if (curtain != null)
        {
            var skinnedMeshRenderer = curtain.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer?.material.color = new Color(1f, 0.5f, 0.5f);
        }

        // Set particles
        var particles = GameObject.Find("Particle System");
        if (particles != null)
        {
            particles.transform.localPosition = new Vector3(-60f, 12f, -4.8f);
            particles.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            particles.transform.localScale = new Vector3(1f, 1.45f, 1f);
            var particleSystem = particles.GetComponent<ParticleSystem>();
            particles.SetActive(false);
            if (particleSystem != null)
            {
                particleSystem.startColor = new(0.4f, 0.1f, 0.1f, 0.5f);
                particleSystem.startLifetime = 15f;
                var emission = particleSystem.emission;
                emission.rateMultiplier = 10;
                emission.rateOverTimeMultiplier = 10;
            }
            particles.SetActive(true);
        }

        // Set 87 logo color
        var quad = GameObject.Find("Quad");
        quad?.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.5f, 0.5f);

        // Set music
        var music = GameObject.Find("cinematic/Audio Source (14)");
        if (music != null)
        {
            var audioSource = music.GetComponent<AudioSource>();
            audioSource.pitch = 0.6f;
        }

        // Set creepy lullaby
        var sound = GameObject.Find("cinematic/Audio Source (12)");
        if (sound != null)
        {
            var audioSource = sound.GetComponent<AudioSource>();
            audioSource.volume = 0.2f;
            audioSource.pitch = 2f;
        }

        // Setup logo
        var fnafTMP = Utils.FindInactive("Canvas/Text (TMP) (4)")?.GetComponent<TextMeshProUGUI>();
        var titleTMP = Utils.FindInactive("Canvas/Text (TMP) (5)")?.GetComponent<TextMeshProUGUI>();
        if (fnafTMP != null && titleTMP != null)
        {
            fnafTMP.transform.localPosition = new(0f, 240f, 0f);
            titleTMP.text += "\nNightmare Mode";
        }

        NightManager.LoadNightUI();
    }

    private static IEnumerator CoSlideModeIn(GameObject mode)
    {
        Vector3 startPos = new(-25f, 40f, 0f);
        Vector3 targetPos = new(-25f, 30f, 0f);
        mode.transform.localPosition = startPos;

        var wait = 5.8f;
        while (wait > 0f)
        {
            wait -= Time.deltaTime;
            yield return null;
        }

        while (mode.transform.localPosition != targetPos)
        {
            mode.transform.localPosition = Vector3.Lerp(mode.transform.localPosition, targetPos, Time.deltaTime * 2.5f);
            yield return null;
        }
        mode.transform.localPosition = targetPos;
    }

    [HarmonyPatch(nameof(MenuScript.Update))]
    [HarmonyPrefix]
    private static bool Update_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        return false;
    }
}