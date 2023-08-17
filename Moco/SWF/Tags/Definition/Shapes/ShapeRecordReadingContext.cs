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
    /// The style reading context.
    /// </summary>
    public StyleReadingContext StyleReadingContext { get; init; }

    /// <summary>
    /// Constructs a new shape record reading context.
    /// </summary>
    /// <param name="numFillBits">The number of fill bits.</param>
    /// <param name="numLineBits">The number of line bits.</param>
    /// <param name="styleCtx">The style reading context.</param>
    public ShapeRecordReadingContext(
        uint numFillBits, 
        uint numLineBits,
        StyleReadingContext styleCtx)
    {
        NumFillBits = numFillBits;
        NumLineBits = numLineBits;
        StyleReadingContext = styleCtx;
    }
}
