using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Definition.Shapes.Records;

/// <summary>
/// A shape record.
/// </summary>
public interface IShapeRecord
{
    /// <summary>
    /// The type of the shape record.
    /// </summary>
    ShapeRecordType Type { get; }
}
