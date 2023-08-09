namespace Moco.SWF.DataTypes;

/// <summary>
/// The RGBA record represents a color as 32-bit red, green, blue and alpha value. An RGBA
/// color with an alpha value of 255 is completely opaque.
/// </summary>
public struct Rgba
{
    /// <summary>
    /// The red channel.
    /// </summary>
    public byte Red { get; set; }

    /// <summary>
    /// The green channel.
    /// </summary>
    public byte Green { get; set; }

    /// <summary>
    /// The blue channel.
    /// </summary>
    public byte Blue { get; set; }

    /// <summary>
    /// The alpha channel.
    /// </summary>
    public byte Alpha { get; set; }
}
