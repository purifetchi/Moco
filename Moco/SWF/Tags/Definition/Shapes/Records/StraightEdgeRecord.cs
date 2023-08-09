using Moco.SWF.DataTypes;
using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Definition.Shapes.Records;

/// <summary>
/// A shape record defining a straight edge.
/// </summary>
public class StraightEdgeRecord : IShapeRecord
{
    /// <inheritdoc/>
    public ShapeRecordType Type { get; } = ShapeRecordType.EdgeRecord;

    /// <summary>
    /// The X delta.
    /// </summary>
    public Twip DeltaX { get; private set; }

    /// <summary>
    /// The Y delta.
    /// </summary>
    public Twip DeltaY { get; private set; }

    /// <summary>
    /// Parses a straight edge record from the binary reader.
    /// </summary>
    /// <param name="br">The binary reader.</param>
    /// <returns>The straight edge record.</returns>
    internal StraightEdgeRecord Parse(ref BitReader br)
    {
        const int numBitsLength = 4;

        // NumBits is always 2 less than the actual number.
        var numBits = br.ReadUnsignedBits(numBitsLength) + 2;

        var generalLineFlag = br.ReadBitFlag();
        var vertLineFlag = !generalLineFlag && br.ReadBitFlag();

        if (generalLineFlag || !vertLineFlag)
            DeltaX = new Twip(br.ReadSignedBits(numBits));

        if (generalLineFlag || vertLineFlag)
            DeltaY = new Twip(br.ReadSignedBits(numBits));

        return this;
    }
}
