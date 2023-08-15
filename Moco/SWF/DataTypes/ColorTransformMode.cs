namespace Moco.SWF.DataTypes;

/// <summary>
/// The mode the color transform tag is operating in.
/// </summary>
[Flags]
public enum ColorTransformMode
{
    /// <summary>
    /// The terms are added to the previous color.
    /// </summary>
    Addition = 1 << 0,

    /// <summary>
    /// The terms are multiplied with the previous color.
    /// </summary>
    Multiplication = 1 << 1,
}
