namespace Moco.SWF.Serialization.Internal;

/// <summary>
/// The record header, preeceding the tag.
/// </summary>
public struct RecordHeader
{
    /// <summary>
    /// The tag type.
    /// </summary>
    public ushort Type { get; set; }

    /// <summary>
    /// The tag length.
    /// </summary>
    public ushort Length { get; set; }
}
