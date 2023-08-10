using Moco.Exceptions;
using Moco.SWF.DataTypes;
using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;
using Moco.SWF.Tags.Definition.Shapes;

namespace Moco.SWF.Tags.Definition;

/// <summary>
/// The DefineShape tag defines a shape for later use by control tags such as PlaceObject.
/// </summary>
public class DefineShape : Tag,
    ICharacterDefinitionTag,
    IVersionedTag
{
    /// <inheritdoc/>
    public override TagType Type => TagType.DefineShape;

    /// <inheritdoc/>
    public override int MinimumVersion => 1;

    /// <inheritdoc/>
    public int Version { get; init; }

    /// <summary>
    /// The id of the shape.
    /// </summary>
    public ushort CharacterId { get; private set; }

    /// <summary>
    /// The shape bounds.
    /// </summary>
    public Rectangle ShapeBounds { get; private set; }

    /// <summary>
    /// The shape with style.
    /// </summary>
    public ShapeWithStyle? ShapeWithStyle { get; private set; }

    /// <summary>
    /// Creates a new define shape tag with the given version.
    /// </summary>
    /// <param name="version">The version.</param>
    public DefineShape(int version = 1)
    {
        Version = version;
    }

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader header)
    {
        CharacterId = reader.GetBinaryReader().ReadUInt16();
        ShapeBounds = reader.ReadRectangleRecord();

        var fillStyles = reader.ReadStyleArray(reader.ReadFillStyle);
        var lineStyles = reader.ReadStyleArray(reader.ReadLineStyle);

        var br = new BitReader(reader.GetBinaryReader());
        var numFillBits = br.ReadUnsignedBits(4);
        var numLineBits = br.ReadUnsignedBits(4);

        var records = reader.ReadShapeRecordsList(new ShapeRecordReadingContext(numFillBits, numLineBits));

        ShapeWithStyle = new ShapeWithStyle(
            fillStyles,
            lineStyles,
            records);

        return this;
    }
}
