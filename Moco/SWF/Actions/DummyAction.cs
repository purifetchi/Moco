using Moco.SWF.Serialization;

namespace Moco.SWF.Actions;

/// <summary>
/// A dummy action.
/// </summary>
public class DummyAction : SwfAction
{
    /// <inheritdoc/>
    public override ActionType Type => _type;

    /// <summary>
    /// The actual type.
    /// </summary>
    private ActionType _type;

    /// <summary>
    /// The length we have to skip.
    /// </summary>
    private int _length;

    /// <summary>
    /// Creates a new dummy action.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="length">The length.</param>
    public DummyAction(
        ActionType type, 
        int length)
    {
        Console.WriteLine($"[SwfReader::ReadAction] Creating dummy for unknown action {type}.");
        _type = type;
        _length = length;
    }

    /// <inheritdoc/>
    public override void Execute(ActionExecutionContext _)
    {
        Console.WriteLine($"[Action::Execute] Dummy action executed for action type {_type}");
    }

    /// <inheritdoc/>
    public override SwfAction Parse(SwfReader reader)
    {
        _ = reader.GetBinaryReader().ReadBytes(_length);
        return this;
    }
}
