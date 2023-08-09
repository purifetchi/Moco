namespace Moco.SWF.Tags.Definition.Shapes;

/// <summary>
/// Context required for reading shape records.
/// </summary>
internal readonly struct ShapeRecordReadingContext
{
    /// <summary>
    /// Number of fill index bits for styles.
    /// </summary>
    public uint NumFillBits { get; init; }

    /// <summary>
    /// Number of line index bits for styles.
    /// </summary>
    public uint NumLineBits { get; init; }

    /// <summary>
    /// Constructs a new shape record reading context.
    /// </summary>
    /// <param name="numFillBits">The number of fill bits.</param>
    /// <param name="numLineBits">The number of line bits.</param>
    public ShapeRecordReadingContext(
        uint numFillBits, 
        uint numLineBits)
    {
        NumFillBits = numFillBits;
        NumLineBits = numLineBits;
    }
}
