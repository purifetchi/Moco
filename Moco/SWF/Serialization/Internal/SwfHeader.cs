using Moco.SWF.DataTypes;

namespace Moco.SWF.Serialization.Internal;

/// <summary>
/// The swf header.
/// </summary>
internal struct SwfHeader
{
    /// <summary>
    /// The swf version.
    /// </summary>
    public byte Version { get; set; }

    /// <summary>
    /// The file length in bytes.
    /// </summary>
    public uint FileLength { get; set; }

    /// <summary>
    /// The frame size in twips.
    /// </summary>
    public Rectangle FrameSize { get; set; }

    /// <summary>
    /// The frame delay in 8.8 fixed number of frames per second.
    /// </summary>
    public float FrameRate { get; set; }

    /// <summary>
    /// Total number of frames in this file.
    /// </summary>
    public ushort FrameCount { get; set; }
}
