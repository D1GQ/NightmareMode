using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Enums;
using NightmareMode.Items.Interfaces;
using NightmareMode.Modules;
using NightmareMode.Monos;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareMode.Managers;

internal static class NightManager
{
    internal static readonly string SummaryNote = "Summary... Watch the music box, Mangle’s aggression, Toy duo’s teamwork, and mask timing. Toy Freddy, Foxy and Balloon Boy are threats, and old Freddy and Mangle are slow to leave! ";

    internal static ITimeEvent? Current => !IsChallengeNight ? CurrentNight : CurrentChallenge;
    internal static INight? CurrentNight;
    internal static IChallenge? CurrentChallenge;
    internal static bool IsChallengeNight;
    internal static int CurrentChallengeId;
    internal static bool InPowerSurge;

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

    internal static void OnNewHour(int hour) => Current?.OnHour(hour);

    internal static void OnHalfHour(int hour) => Current?.OnHalfHour(hour);

    internal static void DelayedNightAction(Action action, float seconds)
    {
        CatchedSingleton<PauseScript>.Instance?.StartCoroutine(CoDelayedNightAction(action, seconds));
    }

    private static IEnumerator CoDelayedNightAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

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
