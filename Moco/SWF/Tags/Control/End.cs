using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Control;

/// <summary>
/// The tag marking the end of the swf file (or a sprite).
/// </summary>
public class End : Tag
{
    /// <inheritdoc/>
    public override TagType Type => TagType.End;

    /// <inheritdoc/>
    public override int MinimumVersion => 1;

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader header)
    {
        return this;
    }
}
