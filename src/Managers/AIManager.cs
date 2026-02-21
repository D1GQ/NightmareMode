using NightmareMode.Modules.AI;

namespace NightmareMode.Managers;

/// <summary>
/// Central manager class that holds references to all active AI controllers in the game.
/// Provides static access to each animatronic's AI script for debugging, monitoring, and interaction purposes.
/// </summary>
internal class AIManager
{
    /// <summary>
    /// Gets or sets the AI controller for Toy Freddy.
    /// </summary>
    internal static ToyFreddyAIWrapper? Toy_FreddyAI;

    /// <summary>
    /// Gets or sets the AI controller for Toy Bonnie.
    /// </summary>
    internal static AnimatronicAIWrapper? Toy_BonnieAI;

    /// <summary>
    /// Gets or sets the AI controller for Toy Chica.
    /// </summary>
    internal static AnimatronicAIWrapper? Toy_ChicaAI;

    /// <summary>
    /// Gets or sets the AI controller for Mangle.
    /// </summary>
    internal static AnimatronicAIWrapper? MangleAI;

    /// <summary>
    /// Gets or sets the AI controller for Balloon Boy.
    /// </summary>
    internal static BBAIWrapper? BalloonBoyAI;

    /// <summary>
    /// Gets or sets the AI controller for The Puppet.
    /// </summary>
    internal static PuppetAIWrapper? PuppetAI;

    /// <summary>
    /// Gets or sets the AI controller for Withered Freddy.
    /// </summary>
    internal static AnimatronicAIWrapper? W_FreddyAI;

    /// <summary>
    /// Gets or sets the AI controller for Withered Bonnie.
    /// </summary>
    internal static AnimatronicAIWrapper? W_BonnieAI;

    /// <summary>
    /// Gets or sets the AI controller for Withered Chica.
    /// </summary>
    internal static AnimatronicAIWrapper? W_ChicaAI;

    /// <summary>
    /// Gets or sets the AI controller for Withered Foxy.
    /// </summary>
    internal static FoxyAIWrapper? W_FoxyAI;
}