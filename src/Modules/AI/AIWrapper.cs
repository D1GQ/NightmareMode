using NightmareMode.Enums;

namespace NightmareMode.Modules.AI;

/// <summary>
/// Represents an abstract wrapper for AI components, providing common functionality and properties
/// </summary>
/// <typeparam name="T">The type of the underlying AI object being wrapped</typeparam>
internal abstract class AIWrapper<T>(T ai)
{
    /// <summary>
    /// Gets the underlying AI instance being wrapped
    /// </summary>
    internal readonly T AI = ai;

    /// <summary>
    /// Gets or sets a value indicating whether this AI is active
    /// </summary>
    public abstract bool Active { get; set; }

    /// <summary>
    /// Gets the type of this AI
    /// </summary>
    public abstract AITypes AIType { get; }

    /// <summary>
    /// Gets or sets the difficulty level of this AI
    /// </summary>
    public abstract int Difficulty { get; set; }

    /// <summary>
    /// Gets or sets the delay before this AI starts its operations
    /// </summary>
    public abstract float StartTimer { get; set; }

    /// <summary>
    /// Retrieves the nickname of the AI
    /// </summary>
    /// <returns>A string representing the AI's nickname, or "???" if not specified</returns>
    public virtual string GetNickName() => "???";
}