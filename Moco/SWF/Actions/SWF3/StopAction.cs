using Moco.SWF.Serialization;

namespace Moco.SWF.Actions.SWF3;

/// <summary>
/// Instructs Moco to stop playing the file at the current frame.
/// </summary>
public class StopAction : SwfAction
{
    /// <inheritdoc/>
    public override ActionType Type => ActionType.Stop;

    /// <inheritdoc/>
    public override void Execute(ActionExecutionContext ctx)
    {
        ctx.TargetTimeline.Active = false;
    }

    /// <inheritdoc/>
    public override SwfAction Parse(SwfReader reader)
    {
        return this;
    }
}
