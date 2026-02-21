using NightmareMode.Enums;

namespace NightmareMode.Modules.AI;

/// <summary>
/// Wrapper for Toy Freddy AI that manages its behavior and interactions with breaker positions
/// </summary>
internal class ToyFreddyAIWrapper(ToyFreddyBrain freddy) : AIWrapper<ToyFreddyBrain>(freddy)
{
    /// <summary>
    /// Gets or sets whether the Toy Freddy AI is active
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
    /// Gets the AI type identifier for Toy Freddy
    /// </summary>
    public override AITypes AIType => AITypes.FreddyAI;

    /// <summary>
    /// Gets or sets the difficulty level for Toy Freddy
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
    /// Gets or sets the initial delay before Toy Freddy starts moving
    /// </summary>
    public override float StartTimer
    {
        get { return AI.StartTimer; }
        set { AI.StartTimer = value; }
    }

    /// <summary>
    /// Gets or sets the time between Toy Freddy's movements
    /// </summary>
    public float MoveTimer
    {
        get { return AI.MoveTimer; }
        set { AI.MoveTimer = value; }
    }

    /// <summary>
    /// Retrieves the localized display name for Toy Freddy
    /// </summary>
    /// <returns>The localized string for "Toy Freddy"</returns>
    public override string GetNickName() => Translator.Get("Character.ToyFreddy");

    /// <summary>
    /// Determines which breaker position Toy Freddy has arrived at
    /// </summary>
    /// <returns>The breaker number (1-4) where Toy Freddy is located, or 0 if not at any breaker</returns>
    public int GetBreakerIndex()
    {
        if (AI.choice1.Arrived) return 1;
        if (AI.choice2.Arrived) return 2;
        if (AI.choice3.Arrived) return 3;
        if (AI.choice4.Arrived) return 4;
        return 0;
    }

    /// <summary>
    /// Checks if Toy Freddy is currently at a specific breaker position
    /// </summary>
    /// <param name="breakerNumber">The breaker number to check (1-4)</param>
    /// <returns>True if Toy Freddy is at the specified breaker, otherwise false</returns>
    public bool IsAtBreaker(int breakerNumber)
    {
        return breakerNumber switch
        {
            1 => AI.choice1.Arrived,
            2 => AI.choice2.Arrived,
            3 => AI.choice3.Arrived,
            4 => AI.choice4.Arrived,
            _ => false
        };
    }

    /// <summary>
    /// Triggers a shutdown at the specified breaker position if Toy Freddy is there
    /// </summary>
    /// <param name="breakerNumber">The breaker number to shutdown (1-4)</param>
    public void ShutdownBreaker(int breakerNumber)
    {
        var breaker = breakerNumber switch
        {
            1 => AI.choice1,
            2 => AI.choice2,
            3 => AI.choice3,
            4 => AI.choice4,
            _ => null
        };

        if (breaker?.Arrived == true)
        {
            breaker.OutageTimer = 0f;
        }
    }

    /// <summary>
    /// Attempts to move Toy Freddy to the next position
    /// </summary>
    /// <param name="force">If true, forces a movement check and shuts down any breakers where Toy Freddy has arrived</param>
    public virtual void TryMoveNextPos(bool force = false)
    {
        if (!AI.Active)
        {
            AI.StartTimer = 0f;
        }
        else if (AI.Moving)
        {
            AI.MoveTimer = 0f;
        }
        else if (force)
        {
            ShutdownIfArrived(AI.choice1);
            ShutdownIfArrived(AI.choice2);
            ShutdownIfArrived(AI.choice3);
            ShutdownIfArrived(AI.choice4);
        }
    }

    /// <summary>
    /// Helper method to shutdown a breaker if Toy Freddy has arrived at it
    /// </summary>
    /// <param name="breaker">The breaker position to check and potentially shutdown</param>
    private void ShutdownIfArrived(ToyFreddyPositionScript breaker)
    {
        if (breaker.Arrived)
            breaker.OutageTimer = 0f;
    }
}