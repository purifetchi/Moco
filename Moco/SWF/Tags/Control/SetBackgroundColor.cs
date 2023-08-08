using Moco.SWF.DataTypes;
using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Control;

/// <summary>
/// The SetBackgroundColor tag sets the background color of the display.
/// </summary>
public class SetBackgroundColor : Tag
{
    /// <inheritdoc/>
    public override TagType Type { get; } = TagType.SetBackgroundColor;

    /// <inheritdoc/>
    public override int MinimumVersion { get; } = 1;

    /// <summary>
    /// Color of the display background.
    /// </summary>
    public Rgb BackgroundColor { get; private set; }

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader _)
    {
        BackgroundColor = reader.ReadRGBRecord();
        return this;
    }
}
