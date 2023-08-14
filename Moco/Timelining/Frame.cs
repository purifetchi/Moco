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
    public IReadOnlyList<Tag> EffectorTags => _frameTags;

    /// <summary>
    /// The list of actions to perform.
    /// </summary>
    public IReadOnlyList<SwfAction> Actions => _actions;

    /// <summary>
    /// The frame tags.
    /// </summary>
    private readonly List<Tag> _frameTags;

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
        _frameTags.Add((tag as Tag)!);
    }

    /// <summary>
    /// Adds an action.
    /// </summary>
    /// <param name="action">The action.</param>
    public void AddAction(SwfAction action)
    {
        _actions.Add(action);
    }
}
