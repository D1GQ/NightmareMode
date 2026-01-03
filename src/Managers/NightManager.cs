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

    internal static void LoadNightUI()
    {
        var firstNight = Utils.FindInactive("Canvas/NightsPage/Night1");
        firstNight?.SetActive(false);

        var night6 = Utils.FindInactive("Canvas/NightsPage/Night6");
        if (night6 != null)
        {
            night6.SetActive(false);
            night6.name = "NightPrefab";
            NightUI.NightPrefab = night6;
        }

        NightUI._allNights.Clear();
        NightUI.Create("Toy Playtime", 1, NightsFlag.None, NightType.Night, GetOriginalNightThumbnail(1));
        NightUI.Create("Old Friends", 2, NightsFlag.Night_1, NightType.Night, GetOriginalNightThumbnail(2));
        NightUI.Create("Reunited", 3, NightsFlag.Night_2, NightType.Night, GetOriginalNightThumbnail(3));
        NightUI.Create("Break In", 4, NightsFlag.Night_3, NightType.Night, GetOriginalNightThumbnail(4));
        NightUI.Create("Happiest Day", 5, NightsFlag.Night_4, NightType.Night, GetOriginalNightThumbnail(5));
        NightUI.Create("The Forgotten", 6, NightsFlag.Night_5, NightType.Night, Utils.LoadSprite("NightmareMode.Resources.Images.night6.png", 100f));

        NightUI.Create("Custom Night", 7, NightsFlag.Night_6, NightType.CustomNight);
        NightUI.Create("Toys Revenge", 1, NightsFlag.Night_6, NightType.Challenge);
        NightUI.Create("Power Outage", 2, NightsFlag.Night_6, NightType.Challenge);
        NightUI.Create("Chaos Shuffle", 3, NightsFlag.Night_6, NightType.Challenge);
        NightUI.Create("Overtime", 4, NightsFlag.Night_6, NightType.Challenge);
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
        var power = Utils.FindInactive("Alive/GAMPLAYCOMPONENTS/Power");
        var powerOut = UnityEngine.Object.Instantiate(power, power?.transform.parent);
        powerOut?.SetActive(true);

        DelayedNightAction(() =>
        {
            UnityEngine.Object.Destroy(powerOut);
        }, delay);
    }
}
