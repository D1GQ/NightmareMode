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
internal class OvertimeChallenge : IChallenge
{
    public bool Completed => DataManager.CompletedChallenges.HasCompletedChallenge(ChallengesFlag.OverTimeChallenge);
    public int ChallengeId => 4;

    private string note = "";
    public void InitChallenge()
    {
        Utils.SetCallNote(Translator.Get("Note.Overtime") + "\n\n" + NightManager.SummaryNote);

        _aiMultiplier = 0;
        _shifts = new bool[4];
        for (int i = 0; i < _shifts.Length; i++)
        {
            _shifts[i] = UnityEngine.Random.value >= 0.5f;
        }
    }

    public int Hours => 9;
    public void OnHour(int hour)
    {
        switch (hour)
        {
            case 12:
                At_12AM();
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                ShiftAI();
                break;
            case 7:
                _aiMultiplier -= 1;
                SetAIAll();
                break;
            case 8:
                _aiMultiplier += 3;
                SetAIAll();
                break;
        }
    }
    public void OnHalfHour(int hour) { }

    private void At_12AM()
    {
        Utils.SetStartTimeAllRandom(5f, 60f);
        AIManager.PuppetAI?.StartTimer = 0f;

        Utils.SetDifficultyAll(0);
        AIManager.PuppetAI?.Difficulty = 10;

        ShiftAI();
    }

    int _aiMultiplier;
    bool[] _shifts = [];
    private void ShiftAI()
    {
        for (int i = 0; i < _shifts.Length; i++)
        {
            _shifts[i] = !_shifts[i];
        }
        _aiMultiplier += 2;

        if (_shifts[0])
        {
            AIManager.Toy_FreddyAI?.Difficulty = GetToyFreddyAI();
            AIManager.W_FreddyAI?.Difficulty = 1;
        }
        else
        {
            AIManager.Toy_FreddyAI?.Difficulty = 1;
            AIManager.W_FreddyAI?.Difficulty = GetWFreddyAI();
        }
        if (_shifts[1])
        {
            AIManager.Toy_BonnieAI?.Difficulty = GetToyBonnieAI();
            AIManager.W_BonnieAI?.Difficulty = 1;
        }
        else
        {
            AIManager.Toy_BonnieAI?.Difficulty = 1;
            AIManager.W_BonnieAI?.Difficulty = GetWBonnieAI();
        }
        if (_shifts[2])
        {
            AIManager.Toy_ChicaAI?.Difficulty = GetToyChicaAI();
            AIManager.W_ChicaAI?.Difficulty = 1;
        }
        else
        {
            AIManager.Toy_ChicaAI?.Difficulty = 1;
            AIManager.W_ChicaAI?.Difficulty = GetWChicaAI();
        }
        if (_shifts[3])
        {
            AIManager.MangleAI?.Difficulty = GetMangleAI();
            AIManager.W_FoxyAI?.Difficulty = 1;
        }
        else
        {
            AIManager.MangleAI?.Difficulty = 1;
            AIManager.W_FoxyAI?.Difficulty = GetWFoxyAI();
        }
        AIManager.BalloonBoyAI?.Difficulty = GetBBAI();
    }

    private void SetAIAll()
    {
        AIManager.Toy_FreddyAI?.Difficulty = GetToyFreddyAI();
        AIManager.Toy_BonnieAI?.Difficulty = GetToyBonnieAI();
        AIManager.Toy_ChicaAI?.Difficulty = GetToyChicaAI();
        AIManager.MangleAI?.Difficulty = GetMangleAI();
        AIManager.BalloonBoyAI?.Difficulty = GetBBAI();
        AIManager.W_FreddyAI?.Difficulty = GetWFreddyAI();
        AIManager.W_BonnieAI?.Difficulty = GetWBonnieAI();
        AIManager.W_ChicaAI?.Difficulty = GetWChicaAI();
        AIManager.W_FoxyAI?.Difficulty = GetWFoxyAI();
    }

    private int GetToyFreddyAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 15);
    private int GetToyBonnieAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 18);
    private int GetToyChicaAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 18);
    private int GetMangleAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 20);
    private int GetBBAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 15);
    private int GetWFreddyAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 15);
    private int GetWBonnieAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 18);
    private int GetWChicaAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 18);
    private int GetWFoxyAI() => Mathf.Clamp(_aiMultiplier + UnityEngine.Random.Range(2, 4), 0, 12);

    public void OnWin()
    {
        DataManager.CompletedChallenges.SetChallengeCompleted(ChallengesFlag.OverTimeChallenge);
    }
}
