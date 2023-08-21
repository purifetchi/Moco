using Moco.SWF.DataTypes;

namespace Moco.SWF.Tags.Definition.Shapes;

/// <summary>
/// The fill style.
/// </summary>
public class FillStyle
{
    /// <summary>
    /// The fill style type.
    /// </summary>
    public FillStyleType Type { get; private set; }

    /// <summary>
    /// Solid fill color with transparency information.
    /// </summary>
    public Rgba Color { get; private set; }

    /// <summary>
    /// The gradient matrix.
    /// </summary>
    public Matrix GradientMatrix { get; private set; }

    /// <summary>
    /// The gradient.
    /// </summary>
    public Gradient? Gradient { get; private set; }

    /// <summary>
    /// ID of bitmap character for fill.
    /// </summary>
    public ushort BitmapId { get; private set; }

    /// <summary>
    /// Matrix for bitmap fill.
    /// </summary>
    public Matrix BitmapMatrix { get; private set; }

    /// <summary>
    /// Constructs a new fill style for the bitmap fill.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="bitmapId">The bitmap id.</param>
    /// <param name="bitmapMatrix">The bitmap matrix.</param>
    public FillStyle(
        FillStyleType type, 
        ushort bitmapId, 
        Matrix bitmapMatrix)
    {
        Type = type;
        BitmapId = bitmapId;
        BitmapMatrix = bitmapMatrix;
    }

    /// <summary>
    /// Constructs a new fill style for the solid fill.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="color">The color.</param>
    public FillStyle(
        FillStyleType type,
        Rgba color)
    {
        Type = type;
        Color = color;
    }

    /// <summary>
    /// Constructs a new fill style for the gradient fill.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="gradientMatrix">The gradient matrix.</param>
    /// <param name="gradient">The gradient.</param>
    public FillStyle(
        FillStyleType type,
        Matrix gradientMatrix,
        Gradient gradient)
    {
        Type = type;
        GradientMatrix = gradientMatrix;
        Gradient = gradient;
    }
}
