using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Control;

/// <summary>
/// Shows a frame and pauses the file for the duration of a single frame.
/// </summary>
public class ShowFrame : Tag
{
    /// <inheritdoc/>
    public override TagType Type => TagType.ShowFrame;

    /// <inheritdoc/>
    public override int MinimumVersion => 1;

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader header)
    {
        return this;
    }
}
