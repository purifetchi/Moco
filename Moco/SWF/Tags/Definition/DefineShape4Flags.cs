namespace Moco.SWF.Tags.Definition;

/// <summary>
/// The DefineShape4 flags.
/// </summary>
[Flags]
public enum DefineShape4Flags : byte
{
    /// <summary>
    /// No flags set. This should always be the value for DefineShape(2/3).
    /// </summary>
    None = 0,

    /// <summary>
    /// If set, the shape contains at least one scaling stroke.
    /// </summary>
    UsesScalingStrokes = 1 << 0,

    /// <summary>
    /// If set, the shape contains at least one non-scaling stroke.
    /// </summary>
    UsesNonScalingStrokes = 1 << 1,

    Reserved1 = 1 << 2,
    Reserved2 = 1 << 3,
    Reserved3 = 1 << 4,
    Reserved4 = 1 << 5,
    Reserved5 = 1 << 6,
    Reserved6 = 1 << 7,
}
