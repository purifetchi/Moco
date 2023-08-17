namespace Moco.SWF.Tags.Definition.Shapes;

/// <summary>
/// Context for reading the style arrays of ShapeWithStyle.
/// </summary>
public readonly struct StyleReadingContext
{
    /// <summary>
    /// Should we read the color fields as RGBA? (DefineShape3)
    /// </summary>
    public bool ReadRGBA { get; init; }

    /// <summary>
    /// Should we read the line style array as LineStyle2? (DefineShape4)
    /// </summary>
    public bool ReadLineStyle2 { get; init; } 

    /// <summary>
    /// Constructs a new style reading context.
    /// </summary>
    /// <param name="readRgba">Should we read the color fields as RGBA and not RGB?</param>
    /// <param name="readLineStyle2">Should we read the line style array as LineStyle2?</param>
    public StyleReadingContext(bool readRgba, bool readLineStyle2)
    {
        ReadRGBA = readRgba;
        ReadLineStyle2 = readLineStyle2;
    }
}
