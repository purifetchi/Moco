using Moco.SWF.Serialization;

namespace Moco.SWF.Actions.SWF3;

/// <summary>
/// Instructs Flash Player to go to the specified frame in the current file.
/// </summary>
public class GoToFrameAction : SwfAction
{
    /// <inheritdoc/>
    public override ActionType Type => ActionType.GotoFrame;

    /// <summary>
    /// The frame index.
    /// </summary>
    public int Frame { get; private set; }

    /// <inheritdoc/>
    public override void Execute(ActionExecutionContext ctx)
    {
        ctx.TargetTimeline.SetFrame(Frame);
    }

    /// <inheritdoc/>
    public override SwfAction Parse(SwfReader reader)
    {
        Frame = reader.GetBinaryReader().ReadUInt16();
        return this;
    }
}
