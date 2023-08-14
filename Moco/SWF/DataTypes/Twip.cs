namespace Moco.SWF.DataTypes;

/// <summary>
/// A twip (TWentIeth of a Pixel).
/// </summary>
public readonly struct Twip
{
    /// <summary>
    /// The scaling factor from twips to logical pixels.
    /// </summary>
    private const float LOGICAL_PIXEL_SCALING_FACTOR = 1 / 20f;

    /// <summary>
    /// The twip value.
    /// </summary>
    public int Value { get; init; }

    /// <summary>
    /// The value in logical pixels.
    /// </summary>
    public float LogicalPixelValue => Value * LOGICAL_PIXEL_SCALING_FACTOR;

    /// <summary>
    /// Constructs a new zero twip.
    /// </summary>
    public static Twip Zero { get; } = new Twip(0);

    /// <summary>
    /// Constructs a twip from an int.
    /// </summary>
    /// <param name="value">The value.</param>
    public Twip(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Adds two twips together.
    /// </summary>
    /// <param name="a">The 1st twip.</param>
    /// <param name="b">The 2nd twip.</param>
    /// <returns>The resulting twip.</returns>
    public static Twip operator+(Twip a, Twip b)
    {
        return new Twip(a.Value + b.Value);
    }
}
