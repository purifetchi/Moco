using Moco.Exceptions;
using Moco.SWF.Actions;
using Moco.SWF.Tags;
using Moco.SWF.Tags.Control;

namespace Moco.Timelining;

/// <summary>
/// A single frame within a timeline. Contains all of the tags that need to be parsed.
/// </summary>
public class Frame
{
    /// <summary>
    /// The list of effector tags.
    /// </summary>
    public IReadOnlyList<IControlTag> EffectorTags => _frameTags;

    /// <summary>
    /// The list of actions to perform.
    /// </summary>
    public IReadOnlyList<SwfAction> Actions => _actions;

    /// <summary>
    /// Does this frame clear the display list when displayed?
    /// </summary>
    public bool ClearsDisplayList { get; set; } = false;

    /// <summary>
    /// The frame tags.
    /// </summary>
    private readonly List<IControlTag> _frameTags;

    /// <summary>
    /// The actions.
    /// </summary>
    private readonly List<SwfAction> _actions;

    /// <summary>
    /// Constructs a blank frame.
    /// </summary>
    public Frame()
    {
        _frameTags = new();
        _actions = new();
    }

    /// <summary>
    /// Adds a tag to the tag list of this frame.
    /// </summary>
    /// <param name="tag">The tag.</param>
    public void AddTag<TTag>(TTag tag)
        where TTag : IControlTag
    {
        _frameTags.Add(tag);
    }

    /// <summary>
    /// Adds an action.
    /// </summary>
    /// <param name="action">The action.</param>
    public void AddAction(SwfAction action)
    {
        _actions.Add(action);
    }

    /// <summary>
    /// Executes the tags that modify the display list.
    /// </summary>
    /// <param name="timeline">The timeline to perform it on.</param>
    public void ExecuteTags(Timeline timeline)
    {
        if (ClearsDisplayList)
            timeline.DisplayList.Clear();

        foreach (var tag in EffectorTags)
        {
            if (tag is IDisplayListAffectingTag effector)
                effector.Execute(timeline.DisplayList);
        }
    }

    /// <summary>
    /// Runs the actions for this frame.
    /// </summary>
    /// <param name="ctx">The execution context.</param>
    public void RunActions(ActionExecutionContext ctx)
    {
        if (Actions.Count < 1)
            return;

        for (ctx.PC = 0; ctx.PC < Actions.Count; ctx.PC++)
        {
            Console.WriteLine($"[Frame::RunActions] Executing action {Actions[ctx.PC].Type} at PC {ctx.PC}");
            Actions[ctx.PC].Execute(ctx);

            if (ctx.Halt)
            {
                Console.WriteLine($"[Frame::RunActions] Action execution halted by action {Actions[ctx.PC].Type}");
                break;
            }
        }
    }
}
