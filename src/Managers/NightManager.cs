using NightmareMode.Attributes;
using NightmareMode.Enums;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;
using NightmareMode.Modules;
using NightmareMode.Monos;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareMode.Managers;

/// <summary>
/// Central manager for night progression, challenge modes, and special events in the game.
/// Handles loading night UI, managing current night/challenge state, and controlling power surge sequences.
/// </summary>
internal static class NightManager
{
    /// <summary>
    /// Gets the summary note prefix text used in the log book.
    /// </summary>
    internal static string SummaryNote => Translator.Get("Note.Summary") + " ";

    /// <summary>
    /// Gets the currently active time event (either a regular night or a challenge).
    /// </summary>
    internal static ITimeEvent? Current => !IsChallengeNight ? CurrentNight : CurrentChallenge;

    /// <summary>
    /// Gets or sets the current regular night instance.
    /// </summary>
    internal static INight? CurrentNight;

    /// <summary>
    /// Gets or sets the current challenge instance.
    /// </summary>
    internal static IChallenge? CurrentChallenge;

    /// <summary>
    /// Gets or sets a value indicating whether the current game is a challenge night.
    /// </summary>
    internal static bool IsChallengeNight;

    /// <summary>
    /// Gets or sets the ID of the currently active challenge.
    /// </summary>
    internal static int CurrentChallengeId;

    /// <summary>
    /// Gets or sets a value indicating whether a power surge event is currently active.
    /// </summary>
    internal static bool InPowerSurge;

    /// <summary>
    /// Loads the night selection UI, creating all night and challenge buttons with their appropriate
    /// thumbnails, unlock requirements, and display names.
    /// </summary>
    internal static void LoadNightUI()
    {
        var firstNight = Utils.FindInactive("Canvas/NightsPage/Night1");
        firstNight?.SetActive(false);

        var night6 = Utils.FindInactive("Canvas/NightsPage/Night6");
        if (night6 != null)
        {
            night6.SetActive(false);
            night6.name = "NightPrefab";
            NightUI.SetNightPrefab(night6);
        }

        NightUI.Reset();
        NightUI.Create(Translator.Get("Night1.Title"), 1, NightsFlag.None, NightType.Night, GetOriginalNightThumbnail(1));
        NightUI.Create(Translator.Get("Night2.Title"), 2, NightsFlag.Night_1, NightType.Night, GetOriginalNightThumbnail(2));
        NightUI.Create(Translator.Get("Night3.Title"), 3, NightsFlag.Night_2, NightType.Night, GetOriginalNightThumbnail(3));
        NightUI.Create(Translator.Get("Night4.Title"), 4, NightsFlag.Night_3, NightType.Night, GetOriginalNightThumbnail(4));
        NightUI.Create(Translator.Get("Night5.Title"), 5, NightsFlag.Night_4, NightType.Night, GetOriginalNightThumbnail(5));
        NightUI.Create(Translator.Get("Night6.Title"), 6, NightsFlag.Night_5, NightType.Night, Utils.LoadSprite("NightmareMode.Resources.Images.night6.png", 100f));

        NightUI.Create(Translator.Get("NightCustom.Title"), 7, NightsFlag.Night_6, NightType.CustomNight);
        NightUI.Create(Translator.Get("Challenge.ToysRevenge.Title"), 1, NightsFlag.Night_6, NightType.Challenge);
        NightUI.Create(Translator.Get("Challenge.PowerOutage.Title"), 2, NightsFlag.Night_6, NightType.Challenge);
        NightUI.Create(Translator.Get("Challenge.ChaosShuffle.Title"), 3, NightsFlag.Night_6, NightType.Challenge);
        NightUI.Create(Translator.Get("Challenge.Overtime.Title"), 4, NightsFlag.Night_6, NightType.Challenge);
    }

    /// <summary>
    /// Extracts the original thumbnail sprite from the game's built-in night selection UI.
    /// Used to preserve the original night thumbnails for the modded night selection screen.
    /// </summary>
    /// <param name="night">The night number (1-5) to extract the thumbnail for.</param>
    /// <returns>The extracted sprite, or null if extraction failed.</returns>
    private static Sprite? GetOriginalNightThumbnail(int night)
    {
        var nightObj = Utils.FindInactive($"Canvas/NightsPage/Night{night}");
        if (nightObj != null)
        {
            var rawImage = nightObj.transform.Find("RawImage").GetComponentInChildren<RawImage>(true);
            if (rawImage != null && rawImage.texture != null)
            {
                return Sprite.Create(
                    (Texture2D)rawImage.mainTexture,
                    new Rect(0, 0, rawImage.texture.width, rawImage.texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
        }

        return null;
    }

    /// <summary>
    /// Initializes the current night or challenge based on the game state.
    /// Called when a night or challenge begins.
    /// </summary>
    internal static void Init()
    {
        if (!IsChallengeNight)
        {
            CurrentNight = RegisterNightAttribute.GetClassInstance(n => n.Night == BrainScript.night);
            CurrentNight?.InitNight();
        }
        else
        {
            CurrentChallenge = RegisterChallengeAttribute.GetClassInstance(n => n.ChallengeId == CurrentChallengeId);
            CurrentChallenge?.InitChallenge();
        }
    }

    /// <summary>
    /// Clears the current night/challenge state and resets related variables.
    /// Called when returning to the main menu or after a night ends.
    /// </summary>
    internal static void Clear()
    {
        CurrentNight = null;
        CurrentChallenge = null;
        IsChallengeNight = false;
        CurrentChallengeId = -1;
        InPowerSurge = false;
        if (BrainScript.night >= 7)
        {
            BrainScript.night = PlayerPrefs.GetInt("night", 1);
        }
    }

    /// <summary>
    /// Triggers the OnHour event for the current night or challenge.
    /// Called when a new hour begins in-game.
    /// </summary>
    /// <param name="hour">The hour number that just began.</param>
    internal static void OnNewHour(int hour) => Current?.OnHour(hour);

    /// <summary>
    /// Triggers the OnHalfHour event for the current night or challenge.
    /// Called at the half-hour mark during gameplay.
    /// </summary>
    /// <param name="hour">The current hour number.</param>
    internal static void OnHalfHour(int hour) => Current?.OnHalfHour(hour);

    /// <summary>
    /// Executes an action after a specified delay during a night.
    /// Uses a Unity coroutine to handle the delayed execution.
    /// </summary>
    /// <param name="action">The action to execute after the delay.</param>
    /// <param name="seconds">The delay duration in seconds.</param>
    internal static void DelayedNightAction(Action action, float seconds)
    {
        CatchedSingleton<PauseScript>.Instance?.StartCoroutine(CoDelayedNightAction(action, seconds));
    }

    /// <summary>
    /// Coroutine that waits for a specified delay and then invokes the given action.
    /// </summary>
    /// <param name="action">The action to invoke after the delay.</param>
    /// <param name="delay">The delay duration in seconds.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private static IEnumerator CoDelayedNightAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    /// <summary>
    /// Triggers a power surge event, sequentially activating spot lights
    /// to create a dramatic lighting effect.
    /// </summary>
    internal static void PowerSurge()
    {
        InPowerSurge = true;
        var lightss = Utils.FindInactive("Alive/GAMPLAYCOMPONENTS/WinLights");
        var winLights = UnityEngine.Object.Instantiate(lightss, lightss?.transform.parent);
        winLights?.transform.Find("Global Volume").gameObject.SetActive(false);
        float wait = 0.1f;
        DelayedNightAction(() =>
        {
            winLights?.transform.Find("Spot Light").gameObject.SetActive(true);
            DelayedNightAction(() =>
            {
                winLights?.transform.Find("Spot Light (1)").gameObject.SetActive(true);
                DelayedNightAction(() =>
                {
                    winLights?.transform.Find("Spot Light (2)").gameObject.SetActive(true);
                    DelayedNightAction(() =>
                    {
                        winLights?.transform.Find("Spot Light (3)").gameObject.SetActive(true);
                        DelayedNightAction(() =>
                        {
                            winLights?.transform.Find("Spot Light (4)").gameObject.SetActive(true);
                            DelayedNightAction(() =>
                            {
                                winLights?.transform.Find("Spot Light (5)").gameObject.SetActive(true);
                                DelayedNightAction(() =>
                                {
                                    winLights?.transform.Find("Spot Light (6)").gameObject.SetActive(true);
                                }, wait);
                            }, wait);
                        }, wait);
                    }, wait);
                }, wait);
            }, wait);
        }, wait);

        DelayedNightAction(() =>
        {
            UnityEngine.Object.Destroy(winLights);
            PowerSurgeOut();
        }, 8f);
    }

    /// <summary>
    /// Triggers a power outage effect after a power surge.
    /// Instantiates a power outage visual effect and refreshes the reflection probe.
    /// </summary>
    /// <param name="delay">Delay before the power outage effect ends. Default is 4 seconds.</param>
    internal static void PowerSurgeOut(float delay = 4f)
    {
        InPowerSurge = true;
        var power = Utils.FindInactive("Alive/GAMPLAYCOMPONENTS/Power");
        var powerOut = UnityEngine.Object.Instantiate(power, power?.transform.parent);
        powerOut?.SetActive(true);

        DelayedNightAction(() =>
        {
            UnityEngine.Object.Destroy(powerOut);
            var ReflectionProbe = GameObject.Find("Reflection Probe");
            if (ReflectionProbe != null)
            {
                var probe = ReflectionProbe.GetComponent<ReflectionProbe>();
                probe?.RenderProbe();
            }

            InPowerSurge = false;
        }, delay);
    }
}