namespace Moco.SWF.DataTypes;

/// <summary>
/// The GRADRECORD record.
/// </summary>
public struct GradientRecord
{
    /// <summary>
    /// The ratio value.
    /// </summary>
    public byte Ratio { get; set; }

    /// <summary>
    /// The color.
    /// </summary>
    public Rgba Color { get; set; }
}
