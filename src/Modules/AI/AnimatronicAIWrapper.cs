using NightmareMode.Enums;
using NightmareMode.Managers;

namespace NightmareMode.Modules.AI;

/// <summary>
/// Wrapper for core animatronic AI that manages movement through position nodes and stage presence
/// </summary>
internal class AnimatronicAIWrapper(AnimatronicAIScript anim) : AIWrapper<AnimatronicAIScript>(anim)
{
    /// <summary>
    /// Gets or sets whether the animatronic AI is enabled
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
    /// Gets the AI type identifier based on which animatronic this wrapper represents
    /// </summary>
    public override AITypes AIType
    {
        get
        {
            if (this == AIManager.Toy_BonnieAI) return AITypes.BonnieAI;
            if (this == AIManager.Toy_ChicaAI) return AITypes.ChicaAI;
            if (this == AIManager.MangleAI) return AITypes.MangleAI;
            if (this == AIManager.W_FreddyAI) return AITypes.WFreddyAI;
            if (this == AIManager.W_BonnieAI) return AITypes.WBonnieAI;
            if (this == AIManager.W_ChicaAI) return AITypes.WChicaAI;
            return AITypes.None;
        }
    }

    /// <summary>
    /// Gets or sets the difficulty level for the animatronic
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
    /// Gets or sets the initial delay before the animatronic starts moving
    /// </summary>
    public override float StartTimer
    {
        get { return AI.StartTimer; }
        set { AI.StartTimer = value; }
    }

    /// <summary>
    /// Retrieves the localized display name for the animatronic
    /// </summary>
    /// <returns>
    /// Localized string based on animatronic type:
    /// - Toy Bonnie, Toy Chica, Mangle
    /// - Withered Freddy, Withered Bonnie, Withered Chica
    /// Returns "???" if type cannot be determined
    /// </returns>
    public override string GetNickName()
    {
        return AIType switch
        {
            AITypes.BonnieAI => Translator.Get("Character.ToyBonnie"),
            AITypes.ChicaAI => Translator.Get("Character.ToyChica"),
            AITypes.MangleAI => Translator.Get("Character.Mangle"),
            AITypes.WFreddyAI => Translator.Get("Character.Freddy"),
            AITypes.WBonnieAI => Translator.Get("Character.Bonnie"),
            AITypes.WChicaAI => Translator.Get("Character.Chica"),
            _ => "???"
        };
    }

    /// <summary>
    /// Moves the animatronic to the position at the specified index in its path
    /// </summary>
    /// <param name="index">The zero-based index of the target position</param>
    public virtual void MoveToPos(int index) => MoveToPos(GetPose(index));

    /// <summary>
    /// Moves the animatronic to a specific position, handling stage departure and position deactivation
    /// </summary>
    /// <param name="positionScript">The target position to move to. If null, only handles stage departure</param>
    public virtual void MoveToPos(AnimatronicPositionScript? positionScript)
    {
        var pos = GetActivePose();
        if (pos != null)
        {
            pos.Static.wait += 1f;
            pos.Model.SetActive(false);
            pos.ACTIVE = false;
        }
        else
        {
            if (!AI.OffStage)
            {
                if (!AI.Withered)
                {
                    AI.Stage.Lower();
                }

                if (AI.Withered || AI.Stage.down)
                {
                    AI.StageModel.SetActive(false);
                    AI.StageStatic.wait += 1f;
                    AI.OffStage = true;
                    AI.StartTimer = 0f;
                }
            }
        }

        positionScript?.Activate();
    }

    /// <summary>
    /// Checks if the animatronic is currently in the office
    /// </summary>
    /// <returns>True if the animatronic's last position has an active office position</returns>
    public virtual bool IsInOffice() => GetPoses().LastOrDefault()?.OfficePosition?.activeInHierarchy == true;

    /// <summary>
    /// Moves the animatronic directly to the office position
    /// </summary>
    public virtual void MoveToOffice()
    {
        MoveToPos(null);
        GetPoses().LastOrDefault()?.OfficePosition?.SetActive(true);
    }

    /// <summary>
    /// Attempts to move the animatronic to the next position in its path
    /// </summary>
    /// <param name="force">If true, forces immediate movement to next position; if false, resets timer at current position</param>
    public virtual void TryMoveNextPos(bool force = false)
    {
        var pos = GetActivePose();
        if (pos != null)
        {
            if (!force)
            {
                pos.Timer = 0;
            }
            else
            {
                pos.NextPos?.Activate();
                pos.Static.wait += 1f;
                pos.Model.SetActive(false);
                pos.ACTIVE = false;
            }
        }
        else
        {
            StartTimer = 0f;
        }
    }

    /// <summary>
    /// Gets the currently active position of the animatronic
    /// </summary>
    /// <returns>The active position script, or null if no position is active</returns>
    public virtual AnimatronicPositionScript? GetActivePose()
    {
        var pos = AI.FirstPosition;
        while (pos != null)
        {
            if (pos.Model.activeInHierarchy)
                return pos;
            pos = pos.NextPos;
        }
        return null;
    }

    /// <summary>
    /// Gets the index of the currently active position
    /// </summary>
    /// <returns>Zero-based index of the active position, or -1 if no position is active</returns>
    public virtual int GetPoseIndex() => GetPoseIndex(GetActivePose());

    /// <summary>
    /// Gets the index of a specific position in the animatronic's path
    /// </summary>
    /// <param name="positionScript">The position to find the index for</param>
    /// <returns>Zero-based index of the position, or -1 if not found or null</returns>
    public virtual int GetPoseIndex(AnimatronicPositionScript? positionScript)
    {
        if (positionScript == null) return -1;

        var pos = AI.FirstPosition;
        int index = 0;
        while (pos != null)
        {
            if (pos == positionScript)
                return index;
            index++;
            pos = pos.NextPos;
        }
        return -1;
    }

    /// <summary>
    /// Gets an array of all positions in the animatronic's path
    /// </summary>
    /// <returns>Array of AnimatronicPositionScript objects in path order</returns>
    public virtual AnimatronicPositionScript[] GetPoses()
    {
        List<AnimatronicPositionScript> poses = [];
        var pos = AI.FirstPosition;
        while (pos != null)
        {
            poses.Add(pos);
            pos = pos.NextPos;
        }
        return [.. poses];
    }

    /// <summary>
    /// Gets the position at the specified index in the animatronic's path
    /// </summary>
    /// <param name="index">The zero-based index of the position to retrieve</param>
    /// <returns>The position script at the specified index, or null if index is invalid or position doesn't exist</returns>
    public virtual AnimatronicPositionScript? GetPose(int index)
    {
        if (index < 0 || AI.FirstPosition == null)
            return null;

        var pos = AI.FirstPosition;
        for (int i = 0; i < index; i++)
        {
            pos = pos.NextPos;
            if (pos == null)
                return null;
        }
        return pos;
    }
}