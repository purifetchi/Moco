namespace Moco.SWF.DataTypes;

/// <summary>
/// Defines a simple transform that can be applied to the color space of a
/// graphic object. 
/// </summary>
public struct ColorTransform
{
    /// <summary>
    /// The color transform mode.
    /// </summary>
    public ColorTransformMode Mode { get; set; }

    /// <summary>
    /// The red addition term.
    /// </summary>
    public int RedAddTerm { get; set; }

    /// <summary>
    /// The green addition term.
    /// </summary>
    public int GreenAddTerm { get; set; }

    /// <summary>
    /// The blue addition term.
    /// </summary>
    public int BlueAddTerm { get; set; }

    /// <summary>
    /// The alpha addition term.
    /// </summary>
    public int AlphaAddTerm { get; set; }

    /// <summary>
    /// The red multiplication term.
    /// </summary>
    public int RedMultTerm { get; set; }

    /// <summary>
    /// The green multiplication term.
    /// </summary>
    public int GreenMultTerm { get; set; }

    /// <summary>
    /// The blue multiplication term.
    /// </summary>
    public int BlueMultTerm { get; set; }

    /// <summary>
    /// The alpha multiplication term.
    /// </summary>
    public int AlphaMultTerm { get; set; }

    /// <summary>
    /// Transform a color with this transform.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The transformed color.</returns>
    public Rgba Transform(Rgba color)
    {
        var newR = color.Red;
        var newG = color.Green;
        var newB = color.Blue;
        var newA = color.Alpha;

        // Multiplication transforms multiply the red, green, blue, and alpha components by an
        // 8.8 fixed-point value.The fixed-point representation of 1.0 is 0x100 or 256 decimal.
        // Therefore, the result of a multiplication operation should be divided by 256.
        if (Mode.HasFlag(ColorTransformMode.Multiplication))
        {
            const float divisor = 256f;

            newR = (byte)((newR * RedMultTerm) / divisor);
            newG = (byte)((newG * GreenMultTerm) / divisor);
            newB = (byte)((newB * BlueMultTerm) / divisor);
            newA = (byte)((newA * AlphaMultTerm) / divisor);
        }

        // Addition transforms add a fixed value (positive or negative) to the red, green, blue, and alpha
        // components of the object being displayed. If the result is greater than 255, the result is
        // clamped to 255.If the result is less than zero, the result is clamped to zero.
        if (Mode.HasFlag(ColorTransformMode.Addition))
        {
            newR = (byte)Math.Min(0, Math.Max(newR + RedAddTerm, byte.MaxValue));
            newG = (byte)Math.Min(0, Math.Max(newG + GreenAddTerm, byte.MaxValue));
            newB = (byte)Math.Min(0, Math.Max(newB + BlueAddTerm, byte.MaxValue));
            newA = (byte)Math.Min(0, Math.Max(newA + AlphaAddTerm, byte.MaxValue));
        }


        return new Rgba
        {
            Red = newR,
            Green = newG,
            Blue = newB,
            Alpha = newA
        };
    }
}
