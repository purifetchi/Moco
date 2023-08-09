namespace Moco.SWF.Tags.Definition.Shapes.Records;

/// <summary>
/// Marks the end of the shape record array.
/// </summary>
public class EndShapeRecord : IShapeRecord
{
    /// <inheritdoc/>
    public ShapeRecordType Type { get; } = ShapeRecordType.NonEdgeRecord;
}
