using Moco.SWF.Serialization;

namespace Moco.SWF.Tags;

/// <summary>
/// An swf tag.
/// </summary>
public abstract class Tag
{
    /// <summary>
    /// The tag type.
    /// </summary>
    public abstract TagType Type { get; }

    /// <summary>
    /// The minimum SWF version for this tag.
    /// </summary>
    public abstract int MinimumVersion { get; }

    /// <summary>
    /// Parses this tag.
    /// </summary>
    /// <param name="reader">The swf reader.</param>
    /// <returns>This tag.</returns>
    internal abstract Tag Parse(SwfReader reader);
}
