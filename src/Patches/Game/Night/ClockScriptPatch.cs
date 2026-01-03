using HarmonyLib;
using NightmareMode.Data;
using NightmareMode.Helpers;
using NightmareMode.Managers;
using NightmareMode.Monos;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NightmareMode.Patches.Game.Night;

[HarmonyPatch(typeof(ClockScript))]
internal class ClockScriptPatch
{
    private static float _12AM => 720f;
    private static float _1Hour => 60f;
    [HarmonyPatch(nameof(ClockScript.Start))]
    [HarmonyPrefix]
    private static bool Start_Prefix(ClockScript __instance)
    {
        if (!NightmarePlugin.ModEnabled) return true;

        NightManager.Init();

        switch (BrainScript.night)
        {
            case 1:
                {
                    __instance.call1.SetActive(true);
                    var audioSourceGlitch = __instance.call1.AddComponent<AudioSourceGlitch>();
                    audioSourceGlitch.doGlitch = false;
                    audioSourceGlitch.ResetAudioSource();
                    var audioSource = __instance.call1.GetComponent<AudioSource>();
                    audioSource.pitch = 0.9f;
                    audioSource.reverbZoneMix = 1.1f;
                    NightManager.DelayedNightAction(() =>
                    {
                        audioSourceGlitch.TriggerPitchDropAndDisable();
                        NightManager.PowerSurgeOut(1.3f);
                    }, 2f);
                }
                break;
            case 2:
                {
                    __instance.call2.SetActive(true);
                    var audioSourceGlitch = __instance.call2.AddComponent<AudioSourceGlitch>();
                    audioSourceGlitch.doGlitch = false;
                    audioSourceGlitch.ResetAudioSource();
                    var audioSource = __instance.call2.GetComponent<AudioSource>();
                    audioSource.pitch = 0.9f;
                    audioSource.reverbZoneMix = 1.1f;
                    NightManager.DelayedNightAction(() =>
                    {
                        audioSourceGlitch.TriggerPitchDropAndDisable();
                        NightManager.PowerSurgeOut(1.3f);
                    }, 2.3f);
                }
                break;
            case 3:
                {
                    __instance.call3.SetActive(true);
                    var audioSource = __instance.call3.GetComponent<AudioSource>();
                    audioSource.pitch = 0.9f;
                    audioSource.reverbZoneMix = 1.1f;
                }
                break;
            case 4:
                {
                    __instance.call4.SetActive(true);
                    var audioSource = __instance.call4.GetComponent<AudioSource>();
                    audioSource.pitch = 0.9f;
                    audioSource.reverbZoneMix = 1.1f;
                }
                break;
        }

        hasHalf = false;
        hasWon = false;
        lastTrackedHour = -1;
        __instance.timer += _12AM;

        var promoFilter = Utils.FindInactive("Alive/PromoRender (2)");
        promoFilter?.SetActive(true);
        var audio = promoFilter?.transform.Find("Camera (1)")?.GetComponent<AudioListener>();
        audio?.enabled = false;

        return false;
    }

    private static int lastTrackedHour;
    private static bool hasWon;
    private static bool hasHalf;
    [HarmonyPatch(nameof(ClockScript.Update))]
    [HarmonyPrefix]
    private static bool Update_Prefix(ClockScript __instance)
    {
        if (!NightmarePlugin.ModEnabled) return true;
        if (NightManager.Current == null) return false;

        __instance.timer += Time.deltaTime / 1.3f;
        if (__instance.timer >= _12AM + _1Hour)
        {
            __instance.timer = _1Hour;
        }
        __instance.DisplayTime();
        int currentHour = (int)__instance.Hours;
        if (currentHour != lastTrackedHour)
        {
            NightManager.OnNewHour(currentHour);
            lastTrackedHour = currentHour;
        }

        int minutes = Mathf.FloorToInt(__instance.timer % 60f);
        bool isCurrentlyInHalfHourWindow = minutes >= 30;
        if (!hasHalf && isCurrentlyInHalfHourWindow)
        {
            hasHalf = true;
            NightManager.OnHalfHour(currentHour);
        }
        else if (hasHalf && !isCurrentlyInHalfHourWindow)
        {
            hasHalf = false;
        }

        if (__instance.timer < _12AM && !hasWon)
        {
            if (__instance.timer >= (_1Hour * NightManager.Current.Hours))
            {
                hasWon = true;
                __instance.anim1.Play();
                __instance.anim2.SetTrigger("Win");
                __instance.anim3.SetTrigger("Off");
                __instance.anim4.SetTrigger("Off");
                __instance.ears.enabled = true;
                Time.timeScale = 1f;
                var array = __instance.windestroy;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetActive(false);
                }
            }
        }

        return false;
    }

    [HarmonyPatch(nameof(ClockScript.WinNight))]
    [HarmonyPrefix]
    private static bool WinNight_Prefix()
    {
        if (!NightmarePlugin.ModEnabled) return true;

        NightManager.Current?.OnWin();

        if (BrainScript.night < 7)
        {
            DataManager.CompletedNights.SetNightCompleted(BrainScript.night.ToNightFlag());
            BrainScript.night++;
            PlayerPrefs.SetInt("night", BrainScript.night);
        }
        else
        {
            BrainScript.night = PlayerPrefs.GetInt("night", 1);
        }
        SceneManager.LoadScene("title");

        return false;
    }
}