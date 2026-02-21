using NightmareMode.Attributes;
using NightmareMode.Data;
using NightmareMode.Enums;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;
using NightmareMode.Managers;
using NightmareMode.Modules;
using UnityEngine;

namespace NightmareMode.Nights.Challenges;

[RegisterChallenge]
internal class ToysRevengeChallenge : IChallenge
{
    public bool Completed => DataManager.CompletedChallenges.HasCompletedChallenge(ChallengesFlag.ToysRevengeChallenge);
    public int ChallengeId => 1;

    private string note = "";
    public void InitChallenge()
    {
        Utils.SetCallNote(Translator.Get("Note.ToysRevenge"));
    }

    public int Hours => 6;
    public void OnHour(int hour)
    {
        switch (hour)
        {
            case 12:
                At_12AM();
                break;
        }
    }
    public void OnHalfHour(int hour) { }

    private void At_12AM()
    {
        Utils.SetStartTimeAllRandom(0f, 10f);
        AIManager.PuppetAI?.StartTimer = 0f;

        Utils.SetDifficultyAll(0);
        AIManager.Toy_FreddyAI?.Difficulty = 20;
        AIManager.Toy_BonnieAI?.Difficulty = 35;
        AIManager.Toy_ChicaAI?.Difficulty = 35;
        AIManager.PuppetAI?.Difficulty = 12;
        AIManager.MangleAI?.Difficulty = 50;

        var bbAudioSource = AIManager.BalloonBoyAI?.AI?.OfficeBB?.GetComponentInChildren<AudioSource>(true);
        if (bbAudioSource != null)
        {
            bbAudioSource.pitch = 0.8f;
            bbAudioSource.volume = 0.2f;
            bbAudioSource.reverbZoneMix = 1.1f;
        }
        AIManager.BalloonBoyAI?.AI?.BBOFFICE();
        CatchedSingleton<MouseFollowScript>.Instance.FrontLight.disabled = true;
        AIManager.BalloonBoyAI?.AI?.OfficeBB?.SetLocalSpace(new Vector3(0.2468f, 0.33f, 4.4938f), null, Quaternion.Euler(0.3128f, 79.3757f, 0f));

        NightManager.DelayedNightAction(() =>
        {
            var mangleHallScript = AIManager.MangleAI?.GetPoses().Last().OfficePosition?.GetComponentInChildren<MangleHallScript>(true);
            if (mangleHallScript != null)
            {
                mangleHallScript.selfanim.speed = 1.38f;
            }
        }, 3f);
    }

    public void OnWin()
    {
        DataManager.CompletedChallenges.SetChallengeCompleted(ChallengesFlag.ToysRevengeChallenge);
    }
}
