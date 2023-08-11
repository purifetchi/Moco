using Moco.Exceptions;
using Moco.SWF.DataTypes;
using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;

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

    /// <summary>
    /// The flags. 
    /// </summary>
    public StyleChangeRecordFlags Flags { get; init; }

    /// <summary>
    /// The move delta X value.
    /// </summary>
    public Twip MoveDeltaX { get; private set; }

    /// <summary>
    /// The move delta Y value.
    /// </summary>
    public Twip MoveDeltaY { get; private set; }

    /// <summary>
    /// Fill 0 Style
    /// </summary>
    public uint FillStyle0 { get; private set; }

    /// <summary>
    /// Fill 1 Style
    /// </summary>
    public uint FillStyle1 { get; private set; }

    /// <summary>
    /// Line Style
    /// </summary>
    public uint LineStyle { get; private set; }

    /// <summary>
    /// Array of new fill styles
    /// </summary>
    public FillStyle[]? FillStyles { get; private set; }

    /// <summary>
    /// Array of new line styles
    /// </summary>
    public LineStyle[]? LineStyles { get; private set; }

    /// <summary>
    /// Number of fill index bits for new styles
    /// </summary>
    public uint NumFillBits { get; private set; }

    /// <summary>
    /// Number of line index bits for new styles
    /// </summary>
    public uint NumLineBits { get; private set; }

    /// <summary>
    /// Constructs a new style change record from the given flags.
    /// </summary>
    /// <param name="flags">The flags.</param>
    public StyleChangeRecord(StyleChangeRecordFlags flags)
    {
        Flags = flags;
    }

    /// <summary>
    /// Parses this style change record from the bit reader.
    /// </summary>
    /// <param name="br">The bit reader.</param>
    /// <param name="ctx">The context.</param>
    /// <returns>The parsed style change record.</returns>
    internal StyleChangeRecord Parse(
        ref BitReader br,
        SwfReader swfReader,
        ref ShapeRecordReadingContext ctx)
    {
        if (Flags.HasFlag(StyleChangeRecordFlags.HasMoveTo))
        {
            const int lengthOfNumBits = 5;

            var bits = br.ReadUnsignedBits(lengthOfNumBits);
            MoveDeltaX = new Twip(br.ReadSignedBits(bits));
            MoveDeltaY = new Twip(br.ReadSignedBits(bits));
        }

        if (Flags.HasFlag(StyleChangeRecordFlags.HasFillStyle0))
            FillStyle0 = br.ReadUnsignedBits(ctx.NumFillBits);

        if (Flags.HasFlag(StyleChangeRecordFlags.HasFillStyle1))
            FillStyle1 = br.ReadUnsignedBits(ctx.NumFillBits);

        if (Flags.HasFlag(StyleChangeRecordFlags.HasLineStyle))
            LineStyle = br.ReadUnsignedBits(ctx.NumLineBits);

        if (Flags.HasFlag(StyleChangeRecordFlags.HasNewStyles))
        {
            FillStyles = swfReader.ReadStyleArray(swfReader.ReadFillStyle);
            LineStyles = swfReader.ReadStyleArray(swfReader.ReadLineStyle);

            // hackity hack
            br = new BitReader(swfReader.GetBinaryReader());
            NumFillBits = br.ReadUnsignedBits(4);
            NumLineBits = br.ReadUnsignedBits(4);

            ctx = new ShapeRecordReadingContext(NumFillBits, NumLineBits);
        }

        return this;
    }
}
