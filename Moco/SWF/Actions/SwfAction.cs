using Moco.SWF.Serialization;

namespace Moco.SWF.Actions;

/// <summary>
/// An SWF3 action.
/// </summary>
public abstract class SwfAction
{
    /// <summary>
    /// The action type.
    /// </summary>
    public abstract ActionType Type { get; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Parses this action.
    /// </summary>
    /// <returns>This action.</returns>
    public abstract SwfAction Parse(SwfReader reader);
}
