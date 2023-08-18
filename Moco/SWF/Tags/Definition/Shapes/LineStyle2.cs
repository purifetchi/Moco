namespace Moco.SWF.Tags.Definition.Shapes;

/// <summary>
/// An extension of LineStyle for DefineShape4.
/// </summary>
public class LineStyle2 : LineStyle
{
    /// <summary>
    /// The LineStyle2 flags.
    /// </summary>
    public LineStyle2Flags Flags { get; set; }

    /// <summary>
    /// The start cap style.
    /// </summary>
    public CapStyle StartCapStyle { get; set; }

    /// <summary>
    /// The join style.
    /// </summary>
    public JoinStyle JoinStyle { get; set; }

    /// <summary>
    /// The end cap style.
    /// </summary>
    public CapStyle EndCapStyle { get; set; }

    /// <summary>
    /// The miter limit factor.
    /// </summary>
    public float MiterLimitFactor { get; set; }

    /// <summary>
    /// The fill style for this stroke.
    /// </summary>
    public FillStyle? FillType { get; set; }
}
