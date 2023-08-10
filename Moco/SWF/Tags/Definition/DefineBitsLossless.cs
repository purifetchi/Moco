using System.IO.Compression;
using Moco.Exceptions;
using Moco.SWF.DataTypes;
using Moco.SWF.Serialization;
using Moco.SWF.Serialization.Internal;

namespace Moco.SWF.Tags.Definition;

/// <summary>
/// Defines a lossless bitmap character that contains RGB bitmap data compressed with ZLIB.
/// </summary>
public class DefineBitsLossless : Tag,
    ICharacterDefinitionTag,
    IVersionedTag
{
    /// <inheritdoc/>
    public override TagType Type => TagType.DefineBitsLossless;

    /// <inheritdoc/>
    public override int MinimumVersion => 2;

    /// <inheritdoc/>
    public int Version { get; init; }

    /// <summary>
    /// The character id.
    /// </summary>
    public ushort CharacterId { get; private set; }

    /// <summary>
    /// The bitmap format.
    /// </summary>
    public byte BitmapFormat { get; private set; }

    /// <summary>
    /// The bitmap width.
    /// </summary>
    public ushort BitmapWidth { get; private set; }

    /// <summary>
    /// The bitmap height.
    /// </summary>
    public ushort BitmapHeight { get; private set; }

    /// <summary>
    /// The bitmap color table size.
    /// </summary>
    public byte BitmapColorTableSize { get; private set; } = 0;

    /// <summary>
    /// The decoded pixel map.
    /// </summary>
    public byte[]? Pixmap { get; private set; }

    /// <summary>
    /// Creates a new define bits lossless tag and sets its version.
    /// </summary>
    /// <param name="version">The version.</param>
    public DefineBitsLossless(int version = 1)
    {
        Version = 1;
    }

    /// <inheritdoc/>
    internal override Tag Parse(SwfReader reader, RecordHeader header)
    {
        const int preludeSize = sizeof(ushort) * 3 + sizeof(byte);
        var br = reader.GetBinaryReader();
        var realPreludeSize = preludeSize;

        CharacterId = br.ReadUInt16();
        BitmapFormat = br.ReadByte();
        BitmapWidth = br.ReadUInt16();
        BitmapHeight = br.ReadUInt16();

        if (BitmapFormat == 3)
        {
            realPreludeSize += sizeof(byte);
            BitmapColorTableSize = br.ReadByte();
        }

        var zlibSize = header.Length - realPreludeSize;
        var data = br.ReadBytes(zlibSize);

        LoadImageData(data);

        return this;
    }

    /// <summary>
    /// Loads the image data.
    /// </summary>
    /// <param name="data">The image data.</param>
    private void LoadImageData(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var zlibStream = new ZLibStream(ms, CompressionMode.Decompress);
        using var reader = new BinaryReader(zlibStream);

        if (Version != 1)
            throw new MocoTodoException(TagType.DefineBitsLossless2, "Not yet supported!");

        var pixels = BitmapFormat switch
        {
            3 => ReadIndexedColorMap(reader),
            _ => null!
        };

        if (pixels is null)
        {
            Console.WriteLine($"[DefineBitsLossless] Unsupported format {BitmapFormat}.");
            return;
        }

        Pixmap = pixels;
    }

    /// <summary>
    /// Reads the indexed color map.
    /// </summary>
    /// <param name="reader">The reader.</param>
    private unsafe byte[] ReadIndexedColorMap(BinaryReader reader)
    {
        // Read the color table.
        var colorTable = stackalloc Rgba[BitmapColorTableSize + 1];
        for (var i = 0; i < BitmapColorTableSize + 1; i++)
        {
            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();

            colorTable[i] = new Rgba
            {
                Red = r,
                Green = g,
                Blue = b,
                Alpha = 255
            };
        }

        // Read the actual color map data.
        const int colorComponents = 4;
        var data = new byte[BitmapWidth * BitmapHeight * colorComponents];

        // Row widths in the pixel data fields of these structures must be rounded up to the next
        // 32 - bit word boundary. For example, an 8-bit image that is 253 pixels wide must be
        // padded out to 256 bytes per line.
        var padding = (4 - (BitmapWidth % 4)) % 4;

        var pixelIndex = 0;
        for (var y = 0; y < BitmapHeight; y++)
        {
            for (var x = 0; x < BitmapWidth; x++)
            {
                var idx = reader.ReadByte();

                var rgb = colorTable[idx];
                data[pixelIndex] = rgb.Red;
                data[pixelIndex + 1] = rgb.Green;
                data[pixelIndex + 2] = rgb.Blue;
                data[pixelIndex + 3] = rgb.Alpha;
                pixelIndex += colorComponents;
            }

            for (var x = 0; x < padding; x++)
                reader.ReadByte();
        }

        return data;
    }
}
