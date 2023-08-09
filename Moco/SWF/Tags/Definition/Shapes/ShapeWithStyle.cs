namespace Moco.SWF.Tags.Definition.Shapes;

/// <summary>
/// The SHAPEWITHSTYLE structure extends the SHAPE structure by including fill style and
/// line style information.
/// </summary>
public class ShapeWithStyle
{
    /// <summary>
    /// Array of fill styles.
    /// </summary>
    public FillStyle[] FillStyles { get; private set; }

    /// <summary>
    /// Array of line styles.
    /// </summary>
    public LineStyle[] LineStyles { get; private set; }

    /// <summary>
    /// Constructs a new shape with style.
    /// </summary>
    /// <param name="fillStyles">The fill styles.</param>
    /// <param name="lineStyles">The line styles.</param>
    public ShapeWithStyle(
        FillStyle[] fillStyles,
        LineStyle[] lineStyles)
    {
        FillStyles = fillStyles;
        LineStyles = lineStyles;
    }
}
