using Moco.Timelining;

namespace Moco.SWF.Actions;

/// <summary>
/// The context passed to actions.
/// </summary>
public class ActionExecutionContext
{
    /// <summary>
    /// The timeline that spawned this action execution context.
    /// </summary>
    public Timeline SourceTimeline { get; init; }

    /// <summary>
    /// The timeline we're performing actions on right now.
    /// </summary>
    public Timeline TargetTimeline { get; set; }

    /// <summary>
    /// The program counter (index into the actions array).
    /// </summary>
    public int PC { get; set; }

    /// <summary>
    /// Constructs a new action execution context.
    /// </summary>
    /// <param name="sourceTimeline">The spawning timeline.</param>
    public ActionExecutionContext(Timeline sourceTimeline)
    {
        SourceTimeline = sourceTimeline;
        TargetTimeline = SourceTimeline;

        PC = 0;
    }
}
