using NightmareMode.Enums;

namespace NightmareMode.Modules.AI;

/// <summary>
/// Wrapper for Foxy AI that manages its active state, difficulty, and animations
/// </summary>
internal class FoxyAIWrapper(FoxyBrainScript foxy) : AIWrapper<FoxyBrainScript>(foxy)
{
    /// <summary>
    /// Gets or sets whether the Foxy AI is currently active
    /// </summary>
    public override bool Active
    {
        get { return AI.Active; }
        set
        {
            AI.Active = value;
        }
    }

    /// <summary>
    /// Gets the AI type identifier for Foxy
    /// </summary>
    public override AITypes AIType => AITypes.FOXYAI;

    /// <summary>
    /// Gets or sets the difficulty level for Foxy
    /// </summary>
    /// <remarks>Setting difficulty to 0 automatically disables the AI</remarks>
    public override int Difficulty
    {
        get { return (int)AI.Difficulty; }
        set
        {
            if (value <= 0)
            {
                SetActive(false);
            }
            AI.enabled = value > 0;
            AI.Difficulty = value;
        }
    }

    /// <summary>
    /// Gets or sets the initial delay before Foxy starts running
    /// </summary>
    public override float StartTimer
    {
        get { return AI.StartTimer; }
        set { AI.StartTimer = value; }
    }

    /// <summary>
    /// Retrieves the localized display name for Foxy
    /// </summary>
    /// <returns>The localized string for "Foxy"</returns>
    public override string GetNickName() => Translator.Get("Character.Foxy");

    /// <summary>
    /// Gets a value indicating whether Foxy is currently in a running state
    /// </summary>
    public bool IsRunning => AI.Active;

    /// <summary>
    /// Sets the active state of Foxy and manages associated animations and timers
    /// </summary>
    /// <param name="active">True to activate Foxy, false to deactivate</param>
    public void SetActive(bool active)
    {
        if (AI.Active == active) return;

        if (!AI.Active && active)
        {
            AI.StartTimer = 0f;
        }
        else if (AI.Active && !active)
        {
            AI.StartTimer = UnityEngine.Random.Range(610, 810) / 10f - AI.Difficulty * 2f;
        }

        AI.Foxy.SetBool("Active", active);
        AI.Active = active;
    }
}