namespace NightmareMode.Interfaces;

/// <summary>
/// Defines the interface for regular night events in the game.
/// Extends ITimeEvent with night-specific functionality for standard gameplay progression.
/// </summary>
internal interface INight : ITimeEvent
{
    /// <summary>
    /// Gets the night number (typically 1-6) that this instance represents.
    /// Used for identification and progression tracking.
    /// </summary>
    int Night { get; }

    /// <summary>
    /// Initializes the night when it begins.
    /// Called when the night is first loaded to set up:
    /// </summary>
    void InitNight();
}