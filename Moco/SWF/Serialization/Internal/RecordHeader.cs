using Moco.SWF.Tags;

namespace Moco.SWF.Serialization.Internal;

/// <summary>
/// The record header, preeceding the tag.
/// </summary>
public struct RecordHeader
{
    /// <summary>
    /// The tag type.
    /// </summary>
    public TagType Type { get; set; }

    /// <summary>
    /// The tag length.
    /// </summary>
    public int Length { get; set; }
}
