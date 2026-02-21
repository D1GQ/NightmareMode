namespace NightmareMode.Interfaces;

/// <summary>
/// Defines the core interface for time-based game events such as nights and challenges.
/// Provides methods for handling time progression and win conditions.
/// </summary>
public interface ITimeEvent
{
    /// <summary>
    /// Gets the total number of hours this time event lasts.
    /// Used to determine when the event should end (typically 6 AM).
    /// </summary>
    int Hours { get; }

    /// <summary>
    /// Called when a new hour begins during the time event.
    /// Implementations should handle hour-specific logic such as:
    /// </summary>
    /// <param name="hour">The hour number that just began (1-6 typically).</param>
    void OnHour(int hour);

    /// <summary>
    /// Called at the half-hour mark during the time event.
    /// Implementations should handle mid-hour logic such as:
    /// </summary>
    /// <param name="hour">The current hour number.</param>
    void OnHalfHour(int hour);

    /// <summary>
    /// Called when the player successfully completes the time event.
    /// Implementations should handle win logic such as:
    /// </summary>
    void OnWin();
}