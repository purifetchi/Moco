namespace Moco.SWF.Tags.Definition.Shapes;

/// <summary>
/// The LineStyle2 flags.
/// </summary>
[Flags]
public enum LineStyle2Flags : ushort
{
    /// <summary>
    /// If set, the stroke will not be closed if the stroke’s last point matches its first point.
    /// Flash Player will apply caps instead of a join.
    /// </summary>
    NoClose = 1 << 0,

    Reserved1 = 1 << 1,
    Reserved2 = 1 << 2,
    Reserved3 = 1 << 3,
    Reserved4 = 1 << 4,
    Reserved5 = 1 << 5,

    /// <summary>
    /// If set, all anchors will be aligned to full pixels.
    /// </summary>
    PixelHinting = 1 << 6,

    /// <summary>
    /// If set, stroke thickness will not scale if the object is scaled vertically.
    /// </summary>
    NoVScale = 1 << 7,

    /// <summary>
    /// If set, stroke thickness will not scale if the object is scaled horizontally.
    /// </summary>
    NoHScale = 1 << 8,

    /// <summary>
    /// If set, fill is defined in FillType.
    /// </summary>
    HasFill = 1 << 9
}
