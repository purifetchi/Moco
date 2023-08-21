namespace Moco.SWF.DataTypes;

/// <summary>
/// Defines a gradient with several stops.
/// </summary>
public class Gradient
{
    /// <summary>
    /// The gradient spread.
    /// </summary>
    public SpreadMode Spread { get; init; }

    /// <summary>
    /// The gradient interpolation.
    /// </summary>
    public InterpolationMode Interpolation { get; init; }

    /// <summary>
    /// The gradient record.
    /// </summary>
    public GradientRecord[] GradientRecords { get; init; } = null!;

    /// <summary>
    /// Constructs a new gradient.
    /// </summary>
    /// <param name="spread">The spread.</param>
    /// <param name="interpolation">The interpolation.</param>
    /// <param name="gradientRecords">The records.</param>
    public Gradient(
        SpreadMode spread, 
        InterpolationMode interpolation, 
        GradientRecord[] gradientRecords)
    {
        Spread = spread;
        Interpolation = interpolation;
        GradientRecords = gradientRecords;
    }
}
