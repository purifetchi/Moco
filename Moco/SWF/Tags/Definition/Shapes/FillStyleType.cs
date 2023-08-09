namespace Moco.SWF.Tags.Definition.Shapes;

/// <summary>
/// The type of the fill.
/// </summary>
public enum FillStyleType : byte
{
    Solid = 0x00,
    LinearGradient = 0x10,
    RadialGradient = 0x12,
    FocalRadialGradient = 0x13,
    RepeatingBitmap = 0x40,
    ClippedBitmap = 0x41,
    NonSmoothedRepeatingBitmap = 0x42,
    NonSmoothedClippedBitmap = 0x43
}
