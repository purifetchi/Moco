using Moco.SWF.DataTypes;

namespace Moco.SWF.Tags.Definition.Shapes;

/// <summary>
/// The line style description.
/// </summary>
public class LineStyle
{
    /// <summary>
    /// The width of the line style.
    /// </summary>
    public ushort Width { get; set; }

    /// <summary>
    /// The color value.
    /// </summary>
    // TODO(pref): Support Rgba for Shape3.
    public Rgb Color { get; set; }
}
