using Moco.SWF.DataTypes;
using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Definition.Shapes.Records;

/// <summary>
/// A shape record describing a Quadratic Bezier curve.
/// </summary>
public class CurvedEdgeRecord : IShapeRecord
{
    /// <inheritdoc/>
    public ShapeRecordType Type { get; } = ShapeRecordType.EdgeRecord;

    /// <summary>
    /// X control point change.
    /// </summary>
    public Twip ControlDeltaX { get; private set; }

    /// <summary>
    /// Y control point change.
    /// </summary>
    public Twip ControlDeltaY { get; private set; }

    /// <summary>
    /// X anchor point change.
    /// </summary>
    public Twip AnchorDeltaX { get; private set; }

    /// <summary>
    /// X anchor point change.
    /// </summary>
    public Twip AnchorDeltaY { get; private set; }

    /// <summary>
    /// Parses a curved edge record from the binary reader.
    /// </summary>
    /// <param name="br">The binary reader.</param>
    /// <returns>The curved edge record.</returns>
    internal CurvedEdgeRecord Parse(ref BitReader br)
    {
        const int numBitsLength = 4;

        // NumBits is always 2 less than the actual number.
        var numBits = br.ReadUnsignedBits(numBitsLength) + 2;

        ControlDeltaX = new Twip(br.ReadSignedBits(numBits));
        ControlDeltaY = new Twip(br.ReadSignedBits(numBits));
        AnchorDeltaX = new Twip(br.ReadSignedBits(numBits));
        AnchorDeltaY = new Twip(br.ReadSignedBits(numBits));

        return this;
    }
}
