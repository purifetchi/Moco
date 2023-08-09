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
    ICharacterDefinitionTag
{
    /// <inheritdoc/>
    public override TagType Type => TagType.DefineShape;

    /// <inheritdoc/>
    public override int MinimumVersion => 1;

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

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader header)
    {
        CharacterId = reader.GetBinaryReader().ReadUInt16();
        ShapeBounds = reader.ReadRectangleRecord();

        // Read the fill styles array.
        // TODO(pref): Support the extended count.
        var fillStyleCount = reader.GetBinaryReader().ReadByte();
        if (fillStyleCount == 0xFF)
            throw new MocoTodoException(TagType.DefineShape, "Extended count not yet supported.");

        var fillStyles = new FillStyle[fillStyleCount];
        for (var i = 0; i < fillStyleCount; i++)
            fillStyles[i] = reader.ReadFillStyle();

        // Read the line styles array.
        // TODO(pref): Support the extended count.
        var lineStyleCount = reader.GetBinaryReader().ReadByte();
        if (lineStyleCount == 0xFF)
            throw new MocoTodoException(TagType.DefineShape, "Extended count not yet supported.");

        var lineStyles = new LineStyle[lineStyleCount];
        for (var i = 0; i < lineStyleCount; i++)
            lineStyles[i] = reader.ReadLineStyle();

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
