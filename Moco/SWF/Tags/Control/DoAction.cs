using Moco.SWF.Actions;
using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Control;

/// <summary>
/// DoAction instructs Flash Player to perform a list of actions when the current frame is
/// complete.
/// </summary>
public class DoAction : Tag,
    IControlTag
{
    /// <inheritdoc/>
    public override TagType Type => TagType.DoAction;

    /// <inheritdoc/>
    public override int MinimumVersion => 3;

    /// <summary>
    /// The actions inside of this tag.
    /// </summary>
    public List<SwfAction>? Actions { get; private set; }

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader header)
    {
        Actions ??= new();

        SwfAction? action;
        do
        {
            action = reader.ReadAction();
            if (action is not null)
                Actions.Add(action);
        } while (action is not null);
        return this;
    }
}
