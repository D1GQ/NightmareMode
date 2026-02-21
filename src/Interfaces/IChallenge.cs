namespace NightmareMode.Interfaces;

/// <summary>
/// Defines the interface for challenge mode events in the game.
/// Extends ITimeEvent with challenge-specific functionality for special gameplay variations.
/// </summary>
internal interface IChallenge : ITimeEvent
{
    /// <summary>
    /// Gets a value indicating whether this challenge has been completed by the player.
    /// Used for tracking progress and unlocking content.
    /// </summary>
    bool Completed { get; }

    /// <summary>
    /// Gets the unique identifier for this challenge.
    /// Used for saving/loading completion status and identifying specific challenge types.
    /// </summary>
    int ChallengeId { get; }

    /// <summary>
    /// Initializes the challenge when it begins.
    /// Called when the challenge is first loaded to set up:
    /// </summary>
    void InitChallenge();
}