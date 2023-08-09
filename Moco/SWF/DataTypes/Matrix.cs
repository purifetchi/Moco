namespace Moco.SWF.DataTypes;

/// <summary>
/// Represents a standard 2x3 transformation matrix of the sort commonly used in 2D graphics.
/// <br />
/// It is used to describe the scale, rotation, and translation of a graphic object. 
/// </summary>
public struct Matrix
{
    /// <summary>
    /// Does the matrix have scaling information?
    /// </summary>
    public bool HasScale { get; set; }

    /// <summary>
    /// The X scale.
    /// </summary>
    public float ScaleX { get; set; }

    /// <summary>
    /// The Y scale.
    /// </summary>
    public float ScaleY { get; set; }

    /// <summary>
    /// Does the matrix have rotation information?
    /// </summary>
    public bool HasRotation { get; set; }

    /// <summary>
    /// First rotate and skew value.
    /// </summary>
    public float RotateSkew0 { get; set; }

    /// <summary>
    /// Second rotate and skew value.
    /// </summary>
    public float RotateSkew1 { get; set; }

    /// <summary>
    /// The X translate value.
    /// </summary>
    public Twip TranslateX { get; set; }

    /// <summary>
    /// The Y translate value.
    /// </summary>
    public Twip TranslateY { get; set; }
}
