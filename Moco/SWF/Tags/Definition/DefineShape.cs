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
    /// Bounds of the shape, excluding strokes.
    /// </summary>
    public Rectangle EdgeBounds { get; private set; }

    /// <summary>
    /// The flags for DefineShape4.
    /// </summary>
    public DefineShape4Flags DefineShape4Flags { get; private set; } = DefineShape4Flags.None;

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

        // NOTE(pref): DefineShape4 specific.
        if (Version == 4)
        {
            const int defineShape4FlagsLength = 8;

            EdgeBounds = reader.ReadRectangleRecord();
            DefineShape4Flags = new BitReader(reader.GetBinaryReader())
                .ReadEnum<DefineShape4Flags>(defineShape4FlagsLength);
        }

        var styleReadingCtx = new StyleReadingContext(
            Version >= 3,
            Version == 4);

        var fillStyles = reader.ReadStyleArray(styleReadingCtx, reader.ReadFillStyle);
        var lineStyles = reader.ReadStyleArray(styleReadingCtx, reader.ReadLineStyle);

        var br = new BitReader(reader.GetBinaryReader());
        var numFillBits = br.ReadUnsignedBits(4);
        var numLineBits = br.ReadUnsignedBits(4);

        var records = reader.ReadShapeRecordsList(
            new ShapeRecordReadingContext(
                numFillBits, 
                numLineBits,
                styleReadingCtx)
            );

        ShapeWithStyle = new ShapeWithStyle(
            fillStyles,
            lineStyles,
            records);

        return this;
    }
}
