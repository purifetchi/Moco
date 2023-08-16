using Moco.SWF.Serialization;

namespace Moco.SWF.Actions.SWF3;

/// <summary>
/// Instructs Moco to start playing at the current frame.
/// </summary>
public class PlayAction : SwfAction
{
    /// <inheritdoc/>
    public override ActionType Type => ActionType.Play;

    /// <inheritdoc/>
    public override void Execute(ActionExecutionContext ctx)
    {
        ctx.TargetTimeline.Active = true;
        ctx.Halt = ctx.SourceTimeline == ctx.TargetTimeline;
    }

    /// <inheritdoc/>
    public override SwfAction Parse(SwfReader reader)
    {
        return this;
    }
}
