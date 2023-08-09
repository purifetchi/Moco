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
    // TODO(pref): Support the RGBA record, used by DefineShape3.
    public Rgb Color { get; private set; }

    // TODO(pref): skipped GradientMatrix, Gradient

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
}
