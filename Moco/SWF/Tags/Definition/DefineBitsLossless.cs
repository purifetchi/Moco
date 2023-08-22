using System.IO.Compression;
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
        Version = version;
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

        var pixels = BitmapFormat switch
        {
            3 => ReadIndexedColorMap(reader),
            5 => Read24BppArgb(reader),
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
            var a = Version == 2 ? reader.ReadByte() : byte.MaxValue;

            colorTable[i] = new Rgba
            {
                Red = r,
                Green = g,
                Blue = b,
                Alpha = a
            };
        }

        // Read the actual color map data.
        const int colorComponents = 4;
        var data = new byte[BitmapWidth * BitmapHeight * colorComponents];

        // Row widths in the pixel data fields of these structures must be rounded up to the next
        // 32 - bit word boundary. For example, an 8-bit image that is 253 pixels wide must be
        // padded out to 256 bytes per line.
        var padding = (4 - (BitmapWidth % 4)) % 4;

        // Reading all of the bytes at once gave us an improvement of ~500%.
        var bytes = reader.ReadBytes(BitmapHeight * (BitmapWidth + padding));

        var pixelIndex = 0;
        var byteArrayIndex = 0;
        for (var y = 0; y < BitmapHeight; y++)
        {
            for (var x = 0; x < BitmapWidth; x++)
            {
                var idx = bytes[byteArrayIndex];

                var rgb = colorTable[idx];
                data[pixelIndex] = rgb.Red;
                data[pixelIndex + 1] = rgb.Green;
                data[pixelIndex + 2] = rgb.Blue;
                data[pixelIndex + 3] = rgb.Alpha;
                pixelIndex += colorComponents;
                byteArrayIndex++;
            }

            for (var x = 0; x < padding; x++)
                byteArrayIndex++;
        }

        return data;
    }

    /// <summary>
    /// Reads a 24-bit/32-bit Argb pixmap data.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>The pixmap.</returns>
    private byte[] Read24BppArgb(BinaryReader reader)
    {
        // Read the actual color map data.
        const int colorComponents = 4;
        var data = new byte[BitmapWidth * BitmapHeight * colorComponents];

        // Reading all of the bytes at once gave us an improvement of ~500%.
        var bytes = reader.ReadBytes(BitmapHeight * BitmapWidth * 4);

        for (var pixelIndex = 0; pixelIndex < bytes.Length; pixelIndex += colorComponents)
        {
            // ARGB -> RGBA
            data[pixelIndex] = bytes[pixelIndex + 1];
            data[pixelIndex + 1] = bytes[pixelIndex + 2];
            data[pixelIndex + 2] = bytes[pixelIndex + 3];
            data[pixelIndex + 3] = bytes[pixelIndex];
        }

        return data;
    }
}
