using NightmareMode.Enums;

namespace NightmareMode.Modules.AI;

/// <summary>
/// Wrapper for Balloon Boy AI that manages his movement between vent and office positions
/// </summary>
internal class BBAIWrapper(BBAIScript bb) : AIWrapper<BBAIScript>(bb)
{
    /// <summary>
    /// Gets or sets whether the Balloon Boy AI is enabled
    /// </summary>
    public override bool Active
    {
        get { return AI.enabled; }
        set
        {
            AI.enabled = value;
        }
    }

    /// <summary>
    /// Gets the AI type identifier for Balloon Boy
    /// </summary>
    public override AITypes AIType => AITypes.BBAI;

    /// <summary>
    /// Gets or sets the difficulty level for Balloon Boy
    /// </summary>
    /// <remarks>Setting difficulty to 0 automatically disables the AI</remarks>
    public override int Difficulty
    {
        get { return (int)AI.Difficulty; }
        set
        {
            AI.enabled = value > 0;
            AI.Difficulty = value;
        }
    }

    /// <summary>
    /// Gets or sets the initial delay before Balloon Boy starts moving
    /// </summary>
    public override float StartTimer
    {
        get { return AI.StartTimer; }
        set { AI.StartTimer = value; }
    }

    /// <summary>
    /// Gets or sets the timer controlling Balloon Boy's movement between positions
    /// </summary>
    public float MoveTimer
    {
        get { return AI.Timer; }
        set { AI.Timer = value; }
    }

    /// <summary>
    /// Retrieves the localized display name for Balloon Boy
    /// </summary>
    /// <returns>The localized string for "Balloon Boy"</returns>
    public override string GetNickName() => Translator.Get("Character.BalloonBoy");

    /// <summary>
    /// Gets a value indicating whether Balloon Boy is currently in the office
    /// </summary>
    public bool IsInOffice => AI.BB2.Active;

    /// <summary>
    /// Gets a value indicating whether Balloon Boy is currently in the vent
    /// </summary>
    public bool IsInVent => AI.BB1.Active;

    /// <summary>
    /// Attempts to move Balloon Boy to the next position in his patrol route
    /// </summary>
    /// <param name="force">If true, forces a position reset regardless of current state</param>
    public virtual void TryMoveNextPos(bool force = false)
    {
        if (!AI.Active)
        {
            AI.StartTimer = 0f;
        }
        else if (!AI.BB1.Active && AI.BB2.Active)
        {
            AI.Timer = 0f;
        }
        else if (force)
        {
            if (AI.BB1.Active)
            {
                AI.BB1.prog = 0f;
                AI.BB1.timer = 0f;
            }
            else if (AI.BB2.Active)
            {
                AI.BB2.prog = 0f;
                AI.BB2.timer = 0f;
            }
        }
    }
}