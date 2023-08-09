namespace Moco.SWF.Tags.Definition.Shapes.Records;

/// <summary>
/// The style change record is also a non-edge record. It can be used to do the following:
/// <br />
/// <br />
/// 1. Select a fill or line style for drawing.
/// <br />
/// 2. Move the current drawing position(without drawing).
/// <br />
/// 3. Replace the current fill and line style arrays with a new set of styles
/// </summary>
public class StyleChangeRecord : IShapeRecord
{
    /// <summary>
    /// The shape record type.
    /// </summary>
    public ShapeRecordType Type { get; } = ShapeRecordType.EdgeRecord;
}
