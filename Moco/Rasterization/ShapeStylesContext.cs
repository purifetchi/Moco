namespace Moco.Rasterization;

/// <summary>
/// The styles context.
/// </summary>
internal struct ShapeStylesContext
{
    /// <summary>
    /// The first fill style.
    /// </summary>
    public int? FillStyle0 { get; set; }

    /// <summary>
    /// The second fill style.
    /// </summary>
    public int? FillStyle1 { get; set; }

    /// <summary>
    /// The line style.
    /// </summary>
    public int? LineStyle { get; set; }
}
