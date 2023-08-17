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

    /// <summary>
    /// The identity matrix, defining a transformation that doesn't change the point.
    /// </summary>
    public static Matrix Identity { get; } = new Matrix
    {
        HasRotation = false,
        HasScale = false,

        ScaleX = 1f,
        ScaleY = 1f,

        RotateSkew0 = 0f,
        RotateSkew1 = 0f,

        TranslateX = Twip.Zero,
        TranslateY = Twip.Zero
    };

    /// <summary>
    /// Combines two matrices together.
    /// <br />
    /// NOTE(pref): No idea if this is a correct impl but it seems to be working.
    /// </summary>
    /// <param name="mat">The other matrix.</param>
    /// <returns>The combined matrix.</returns>
    public Matrix Combine(Matrix mat)
    {
        return new Matrix
        {
            HasRotation = HasRotation || mat.HasRotation,
            HasScale = HasScale || mat.HasScale,

            TranslateX = TranslateX + mat.TranslateX,
            TranslateY = TranslateY + mat.TranslateY,

            RotateSkew0 = RotateSkew0 + mat.RotateSkew0,
            RotateSkew1 = RotateSkew1 + mat.RotateSkew1,

            ScaleX = ScaleX * mat.ScaleX,
            ScaleY = ScaleY * mat.ScaleY
        };
    }
}
