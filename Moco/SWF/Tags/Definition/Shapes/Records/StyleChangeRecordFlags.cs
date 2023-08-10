namespace Moco.SWF.Tags.Definition.Shapes.Records;

/// <summary>
/// Flags for the style change record.
/// </summary>
[Flags]
public enum StyleChangeRecordFlags : byte
{
    /// <summary>
    /// Not an actual flag, but if they're all zeroes, this is an end marker.
    /// </summary>
    IsEndMarker = 0x00,

    /// <summary>
    /// Style change has new styles.
    /// </summary>
    HasNewStyles = 1 << 3,

    /// <summary>
    /// Style change changes the line style.
    /// </summary>
    HasLineStyle = 1 << 3,

    /// <summary>
    /// Style change changes fill style 1.
    /// </summary>
    HasFillStyle1 = 1 << 2,

    /// <summary>
    /// Style change changes fill style 0.
    /// </summary>
    HasFillStyle0 = 1 << 1,

    /// <summary>
    /// Has the move to flag.
    /// </summary>
    HasMoveTo = 1 << 0,
}
