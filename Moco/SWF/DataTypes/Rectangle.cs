namespace Moco.SWF.DataTypes;

/// <summary>
/// A rectangle value represents a rectangular region defined by a minimum x- and y-coordinate
/// position and a maximum x- and y-coordinate position.
/// </summary>
public struct Rectangle
{
    /// <summary>
    /// The minimum x coordinate.
    /// </summary>
    public Twip XMin { get; set; }

    /// <summary>
    /// The maximum x coordinate.
    /// </summary>
    public Twip XMax { get; set; }

    /// <summary>
    /// The minimum y coordinate.
    /// </summary>
    public Twip YMin { get; set; }

    /// <summary>
    /// The maximum y coordinate.
    /// </summary>
    public Twip YMax { get; set; }
}
