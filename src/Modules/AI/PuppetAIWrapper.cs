using NightmareMode.Enums;

namespace NightmareMode.Modules.AI;

/// <summary>
/// Wrapper for The Puppet AI that manages its behavior and difficulty settings
/// </summary>
internal class PuppetAIWrapper(PuppetScript puppet) : AIWrapper<PuppetScript>(puppet)
{
    /// <summary>
    /// Gets or sets whether The Puppet AI is active
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
    /// Gets the AI type identifier for The Puppet
    /// </summary>
    public override AITypes AIType => AITypes.PuppetAI;

    /// <summary>
    /// Gets or sets the difficulty level for The Puppet
    /// </summary>
    /// <remarks>Setting difficulty to 0 automatically disables the AI</remarks>
    public override int Difficulty
    {
        get { return AI.dif; }
        set
        {
            AI.enabled = value > 0;
            AI.dif = value;
        }
    }

    /// <summary>
    /// Gets or sets the initial delay before The Puppet starts its operations
    /// </summary>
    public override float StartTimer
    {
        get { return AI.start; }
        set { AI.start = value; }
    }

    /// <summary>
    /// Retrieves the localized display name for The Puppet
    /// </summary>
    /// <returns>The localized string for "The Puppet"</returns>
    public override string GetNickName() => Translator.Get("Character.Puppet");
}