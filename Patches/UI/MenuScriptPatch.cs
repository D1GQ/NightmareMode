using HarmonyLib;
using NightmareMode.Data;
using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Enums;
using NightmareMode.Managers;
using NightmareMode.Monos;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NightmareMode.Patches.UI;

[HarmonyPatch(typeof(MenuScript))]
internal class MenuScriptPatch
{
    private static List<GameObject?> _allNights = [];
    private static GameObject? _night6Old;

    [HarmonyPatch(nameof(MenuScript.Start))]
    [HarmonyPrefix]
    private static void Start_Prefix()
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
                textTMP.SetText(NightmarePlugin.ModEnabled ? "<#B2B2B2>(Vanilla)</color>" : "<#CC0000>(Nightmare)</color>");
            }
            if (NightmarePlugin.ModEnabled)
                mode.transform.localPosition = new Vector3(-27f, 31f, 0f);
            else
                mode.transform.localPosition = new Vector3(-25.5f, 31f, 0f);
            var button = mode.GetComponent<Button>();
            if (button != null)
            {
                button.onClick = new();
                button.onClick.AddListener(NightmarePlugin.SwitchMode);
            }
        }

        if (!NightmarePlugin.ModEnabled) return;

        /*
        var Static = Utils.FindInactive("Canvas/Image");
        if (Static != null)
        {
            Static.SetActive(true);
            Static.GetComponent<Image>().color = new(1f, 0f, 0f, 0.01f);
        }
        */


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
        if (quad != null)
        {
            quad.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.5f, 0.5f);
        }

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
            fnafTMP.transform.localPosition = new(0f, 270f, 0f);
            titleTMP.text += "\nNightmare Mode";
        }

        // Setup nights ui
        _allNights.Clear();
        _night6Old = Utils.FindInactive("Canvas/Night6");
        if (_night6Old != null)
        {
            _night6Old.name = "Night6 (OLD)";
        }

        NightManager.LoadNightUI();
    }

    [HarmonyPatch(nameof(MenuScript.Update))]
    [HarmonyPrefix]
    private static bool Update_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        return false;
    }

    internal static void CreateNight(string title, int nightOrChallenge, NightsFlag requiredNight, NightType nightType, Sprite? sprite = null)
    {
        var prefab = Utils.FindInactive("Canvas/Night5");
        if (prefab != null)
        {
            var newNightObj = UnityEngine.Object.Instantiate(prefab, prefab.transform.parent);
            newNightObj.name = nightType != NightType.Challenge ? $"Night{nightOrChallenge}" : $"ChallengeNight({title})";
            SetupNextButton(newNightObj);
            _allNights.Add(newNightObj);
            SetTitleForNight(newNightObj, title);
            LockNight(newNightObj, requiredNight);
            RecolorNightImage(newNightObj);

            var completedTMP = newNightObj.transform.Find("Completed").GetComponent<TextMeshProUGUI>();
            if (completedTMP != null)
                completedTMP.transform.localPosition = new(0f, 5f, 0f);
            var nightNum = newNightObj.transform.Find("Text (TMP) (1)")?.GetComponentInChildren<TextMeshProUGUI>(true);
            if (nightNum != null)
            {
                if (nightType != NightType.Challenge)
                {
                    nightNum.SetText($"(Night {nightOrChallenge})");

                    var inight = RegisterNightAttribute.GetClassInstance(c => c.Night == nightOrChallenge);
                    if (inight != null && DataManager.CompletedNights.HasCompletedNight(inight.Night.ToNightFlag()))
                        completedTMP?.SetText("<#00FF20>(Completed)</color>");
                    else
                        completedTMP?.SetText("");
                }
                else
                {
                    nightNum.enableWordWrapping = false;
                    nightNum.SetText($"(Challenge Night)");

                    if (RegisterChallengeAttribute.GetClassInstance(c => c.ChallengeId == nightOrChallenge)?.Completed == true)
                        completedTMP?.SetText("<#00FF20>(Completed)</color>");
                    else
                        completedTMP?.SetText("");
                }
            }
            var play = newNightObj.transform.Find("Play");
            if (play != null)
            {
                var playButton = play.GetComponentInChildren<Button>();
                playButton.onClick = new();

                playButton.onClick.AddListener(() =>
                {
                    newNightObj.SetActive(false);
                    CatchedSingleton<LoadingTip>.Instance.gameObject.SetActive(true);
                    if (nightType != NightType.Challenge)
                        LoadNight(nightOrChallenge);
                    else
                        LoadChallenge(nightOrChallenge);
                });

                if (nightType == NightType.CustomNight)
                {
                    play.transform.localPosition = new Vector3(0f, -15f, 0f);
                    UnityEngine.Object.Destroy(newNightObj.transform.Find("RawImage").gameObject);
                    UnityEngine.Object.Destroy(newNightObj.transform.Find("RawImage (1)").gameObject);
                    CustomNightManager.LoadMenu(newNightObj);
                    return;
                }
            }

            if (nightType == NightType.Challenge)
            {
                sprite = Utils.LoadSprite("NightmareMode.Resources.Images.challenge.png", 100f);
            }

            if (sprite != null)
            {
                var rawImage = newNightObj.transform.Find("RawImage").GetComponentInChildren<RawImage>(true);
                if (rawImage != null)
                {
                    rawImage.texture = sprite.texture;
                }
            }
        }
    }

    private static void SetupNextButton(GameObject nightObj)
    {
        var buttonForLastNightNext = _allNights.Last()?.transform?.Find("Forward")?.GetComponentInChildren<Button>(true);
        if (buttonForLastNightNext != null)
        {
            buttonForLastNightNext.onClick = new();
            buttonForLastNightNext.onClick.AddListener(() =>
            {
                buttonForLastNightNext.transform.parent.gameObject.SetActive(false);
                nightObj.SetActive(true);
            });



            var buttonForNightPrev = nightObj.transform.Find("Back")?.GetComponentInChildren<Button>(true);
            if (buttonForNightPrev != null)
            {
                buttonForNightPrev.onClick = new();
                buttonForNightPrev.onClick.AddListener(() =>
                {
                    nightObj.SetActive(false);
                    buttonForLastNightNext.transform.parent.gameObject.SetActive(true);
                });
            }

            var buttonForNight6OldPrev = _night6Old?.transform.Find("Back")?.GetComponentInChildren<Button>(true);
            if (buttonForNight6OldPrev != null)
            {
                buttonForNight6OldPrev.onClick = new();
                buttonForNight6OldPrev.onClick.AddListener(() =>
                {
                    _night6Old?.SetActive(false);
                    nightObj.SetActive(true);
                });
            }
        }

        var buttonForNightNext = nightObj.transform.Find("Forward")?.GetComponentInChildren<Button>(true);
        if (buttonForNightNext != null)
        {
            buttonForNightNext.onClick = new();
            buttonForNightNext.onClick.AddListener(() =>
            {
                nightObj.SetActive(false);
                _night6Old?.SetActive(true);
            });
        }
    }


    internal static void SetupNightUi(int night, GameObject? nightObj, string title, NightsFlag requiredNight = NightsFlag.None)
    {
        _allNights.Add(nightObj);
        LockNight(nightObj, requiredNight);
        SetTitleForNight(nightObj, title);
        RecolorNightImage(nightObj);

        TextMeshProUGUI? completedTMP = null;
        var text = nightObj?.transform.Find("Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            completedTMP = UnityEngine.Object.Instantiate(text, text.transform.parent);
            completedTMP.transform.localPosition = new(0f, 5f, 0f);
            completedTMP.name = "Completed";
            completedTMP.color = new(1f, 1f, 1f, 0.35f);
            completedTMP.SetText("");
        }

        var inight = RegisterNightAttribute.GetClassInstance(c => c.Night == night);
        if (inight != null && DataManager.CompletedNights.HasCompletedNight(inight.Night.ToNightFlag()))
        {
            completedTMP?.SetText("<#00FF20>(Completed)</color>");
        }
    }

    private static void LockNight(GameObject? nightObj, NightsFlag requiredNight = NightsFlag.None)
    {
        if (_allNightsUnlocked) return;

        var play = nightObj?.transform.Find("Play");
        if (requiredNight != NightsFlag.None && !DataManager.CompletedNights.HasCompletedNight(requiredNight))
        {
            if (play != null)
            {
                play.GetComponent<Button>().enabled = false;
                play.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.1604f, 0.1604f, 0.1604f);
            }
        }
        else
        {
            if (play != null)
            {
                play.GetComponent<Button>().enabled = true;
                play.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }
    }

    private static void SetTitleForNight(GameObject? nightObj, string title)
    {
        if (nightObj == null) return;

        var titleObj = nightObj.transform.Find("title");
        if (titleObj != null)
        {
            var titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
            titleTMP?.SetText(title);
            var nightText = nightObj.transform.Find("Text (TMP) (1)");
            nightText.transform.localPosition = new(0f, 25f, 0f);
            return;
        }

        var text = nightObj.transform.Find("Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            var titleTMP = UnityEngine.Object.Instantiate(text, nightObj.transform);
            titleTMP.name = "title";
            titleTMP.fontSize = 10;
            titleTMP.SetText(title);
            titleTMP.transform.localPosition = new(0f, 20f, 0f);
            text.transform.localPosition = new(0f, 25f, 0f);
            text.text = $"({text.text})";
        }
    }

    private static void RecolorNightImage(GameObject? nightObj)
    {
        if (nightObj == null) return;

        var rawImage = nightObj.transform.Find("RawImage").GetComponent<RawImage>();
        if (rawImage != null)
        {
            rawImage.color = new Color(1f, 0.5f, 0.5f);
        }
    }

    private static bool _allNightsUnlocked;
    internal static void UnloadAllNights()
    {
        if (_allNightsUnlocked) return;
        foreach (var night in _allNights)
        {
            LockNight(night);
        }
        _allNightsUnlocked = true;
    }

    [HarmonyPatch(nameof(MenuScript.Night1))]
    [HarmonyPrefix]
    private static bool Night1_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        LoadNight(1);
        return false;
    }
    [HarmonyPatch(nameof(MenuScript.Night2))]
    [HarmonyPrefix]
    private static bool Night2_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        LoadNight(2);
        return false;
    }
    [HarmonyPatch(nameof(MenuScript.Night3))]
    [HarmonyPrefix]
    private static bool Night3_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        LoadNight(3);
        return false;
    }
    [HarmonyPatch(nameof(MenuScript.Night4))]
    [HarmonyPrefix]
    private static bool Night4_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        LoadNight(4);
        return false;
    }
    [HarmonyPatch(nameof(MenuScript.Night5))]
    [HarmonyPrefix]
    private static bool Night5_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        LoadNight(5);
        return false;
    }

    private static void LoadNight(int night)
    {
        if (night == 1)
        {
            PlayerPrefs.SetInt("ending", 0);
        }

        if (night < 7)
        {
            PlayerPrefs.SetInt("night", night);
        }
        BrainScript.night = night;
        GameObject.Find("cinematic").SetActive(false);
        SceneManager.LoadScene("Nights");
    }

    private static void LoadChallenge(int challengeId)
    {
        NightManager.IsChallengeNight = true;
        NightManager.CurrentChallengeId = challengeId;
        BrainScript.night = 8;
        GameObject.Find("cinematic").SetActive(false);
        SceneManager.LoadScene("Nights");
    }
}