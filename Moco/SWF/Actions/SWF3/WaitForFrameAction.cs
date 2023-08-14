using Moco.SWF.Serialization;

namespace Moco.SWF.Actions.SWF3;

/// <summary>
/// Instructs Moco to wait until the specified frame; otherwise skips
/// the specified number of actions.
/// </summary>
public class WaitForFrameAction : SwfAction
{
    /// <inheritdoc/>
    public override ActionType Type => ActionType.WaitForFrame;

    /// <summary>
    /// The frame.
    /// </summary>
    public int Frame { get; private set; }

    /// <summary>
    /// The amount of frames to skip if it's not loaded.
    /// </summary>
    public int SkipCount { get; private set; }

    /// <inheritdoc/>
    public override void Execute(ActionExecutionContext ctx)
    {
        if (ctx.TargetTimeline.Frames.Count >= Frame)
        {
            Console.WriteLine("[WaitForFrameAction] The frame is loaded :)");
            return;
        }

        Console.WriteLine("[WaitForFrameAction] The frame is not loaded :(");
        ctx.PC += SkipCount;
    }

    /// <inheritdoc/>
    public override SwfAction Parse(SwfReader reader)
    {
        var br = reader.GetBinaryReader();
        Frame = br.ReadUInt16();
        SkipCount = br.ReadByte();

        return this;
    }
}
