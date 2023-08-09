namespace Moco.SWF.Tags.Definition.Shapes.Records;

/// <summary>
/// The type of the shape record.
/// </summary>
public enum ShapeRecordType : byte
{
    /// <summary>
    /// A non-edge record.
    /// </summary>
    NonEdgeRecord = 0x00,

    /// <summary>
    /// An edge record.
    /// </summary>
    EdgeRecord = 0x01,
}
