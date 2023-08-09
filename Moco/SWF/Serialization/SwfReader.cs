using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using Moco.SWF.DataTypes;
using Moco.SWF.Serialization.Internal;
using Moco.SWF.Tags;
using Moco.SWF.Tags.Control;
using Moco.SWF.Tags.Definition;
using Moco.SWF.Tags.Definition.Shapes;

namespace Moco.SWF.Serialization;

/// <summary>
/// A .swf file reader.
/// </summary>
public class SwfReader : IDisposable
{
    /// <summary>
    /// The base stream.
    /// </summary>
    private readonly Stream _baseStream;

    /// <summary>
    /// The reader stream.
    /// </summary>
    private readonly Stream _readerStream;

    /// <summary>
    /// The binary reader.
    /// </summary>
    private readonly BinaryReader _reader;

    /// <summary>
    /// Constructs a new swf reader for a file.
    /// </summary>
    /// <param name="filename"></param>
    public SwfReader(string filename)
    {
        var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        _baseStream = fs;
        _readerStream = ConstructUnderlyingStream(fs);
        _reader = new BinaryReader(_readerStream);
    }

    /// <summary>
    /// Constructs the underlying stream from a given stream.
    /// </summary>
    /// <param name="stream">The stream to construct the underlying stream from.</param>
    /// <returns>The underlying stream..</returns>
    private unsafe Stream ConstructUnderlyingStream(Stream stream)
    {
        const int magicByteSize = 3;
        Span<byte> magic = stackalloc byte[sizeof(int)];
        magic.Clear();

        stream.Read(magic[..magicByteSize]);

        var compression = (SwfCompressionMode)BitConverter.ToInt32(magic);
        return compression switch
        {
            SwfCompressionMode.None => stream,
            SwfCompressionMode.Zlib => new ZLibStream(stream, CompressionMode.Decompress),
            SwfCompressionMode.Lzma => throw new NotSupportedException("Lzma compression not yet supported."),
            _ => throw new NotAnSwfException()
        };
    }

    /// <summary>
    /// Reads the swf file.
    /// </summary>
    /// <returns>The swf object.</returns>
    public Swf ReadSwf()
    {
        var swf = new Swf(ReadHeader());

        while (true)
        {
            try
            {
                swf.AddTag(ReadTag());
            }
            catch
            {
                break;
            }
        }

        return swf;
    }

    /// <summary>
    /// Reads the rectangle record.
    /// </summary>
    /// <returns>The rectangle record.</returns>
    internal Rectangle ReadRectangleRecord()
    {
        const int sizeOfNbits = 5;

        var br = new BitReader(_reader);
        var nbits = br.ReadUnsignedBits(sizeOfNbits);
        return new Rectangle
        {
            XMin = new Twip(br.ReadSignedBits(nbits)),
            XMax = new Twip(br.ReadSignedBits(nbits)),
            YMin = new Twip(br.ReadSignedBits(nbits)),
            YMax = new Twip(br.ReadSignedBits(nbits)),
        };
    }

    /// <summary>
    /// Reads an RGB record.
    /// </summary>
    /// <returns>The RGB record.</returns>
    internal Rgba ReadRGBRecord()
    {
        return new Rgba
        {
            Red = _reader.ReadByte(),
            Green = _reader.ReadByte(),
            Blue = _reader.ReadByte(),
            Alpha = 255
        };
    }

    /// <summary>
    /// Reads an RGBA record.
    /// </summary>
    /// <returns>The RGBA record.</returns>
    internal Rgba ReadRGBARecord()
    {
        return new Rgba
        {
            Red = _reader.ReadByte(),
            Green = _reader.ReadByte(),
            Blue = _reader.ReadByte(),
            Alpha = _reader.ReadByte()
        };
    }

    /// <summary>
    /// Reads the record header for a tag.
    /// </summary>
    /// <returns>The record header.</returns>
    internal RecordHeader ReadRecordHeader()
    {
        const int shortHeaderTypeBitMask = 0b1111111111000000;
        const int shortHeaderLengthBitMask = ~shortHeaderTypeBitMask;

        // The long tag header consists of
        // a short tag header with a length of 0x3f, followed by a 32-bit length
        const short longFormatHeaderMagic = 0x3F;

        var value = _reader.ReadUInt16();
        var type = (TagType)((value & shortHeaderTypeBitMask) >> 6);
        var length = value & shortHeaderLengthBitMask;

        // Check if we're dealing with a long format record header.
        if (length == longFormatHeaderMagic)
            length = _reader.ReadInt32();

        return new RecordHeader
        {
            Type = type,
            Length = length
        };
    }

    /// <summary>
    /// Reads a single matrix record.
    /// </summary>
    /// <returns>The matrix record.</returns>
    internal Matrix ReadMatrixRecord()
    {
        const int bitSizeForXBitsFields = 5;

        var br = new BitReader(_reader);

        var hasScale = br.ReadBit();
        float scaleX = 1f, scaleY = 1f;
        if (hasScale == 1)
        {
            var nScaleBits = br.ReadUnsignedBits(bitSizeForXBitsFields);
            scaleX = br.ReadFloatingBits(nScaleBits);
            scaleY = br.ReadFloatingBits(nScaleBits);
        }

        var hasRotate = br.ReadBit();
        float rotateSkew0 = 0f, rotateSkew1 = 0f;
        if (hasRotate == 1)
        {
            var nRotateBits = br.ReadUnsignedBits(bitSizeForXBitsFields);
            rotateSkew0 = br.ReadFloatingBits(nRotateBits);
            rotateSkew1 = br.ReadFloatingBits(nRotateBits);
        }

        var nTranslateBits = br.ReadUnsignedBits(bitSizeForXBitsFields);
        var translateX = new Twip(br.ReadSignedBits(nTranslateBits));
        var translateY = new Twip(br.ReadSignedBits(nTranslateBits));

        return new Matrix
        {
            HasScale = hasScale == 1,
            ScaleX = scaleX,
            ScaleY = scaleY,

            HasRotation = hasRotate == 1,
            RotateSkew0 = rotateSkew0,
            RotateSkew1 = rotateSkew1,

            TranslateX = translateX,
            TranslateY = translateY
        };
    }

    /// <summary>
    /// Reads a single fill style.
    /// </summary>
    /// <returns>The fill style.</returns>
    internal FillStyle ReadFillStyle()
    {
        var type = (FillStyleType)_reader.ReadByte();

        if (type != FillStyleType.RepeatingBitmap &&
            type != FillStyleType.ClippedBitmap &&
            type != FillStyleType.NonSmoothedRepeatingBitmap &&
            type != FillStyleType.NonSmoothedClippedBitmap)
        {
            throw new NotSupportedException($"Fill type {type} is not yet supported.");
        }

        var bitmapId = _reader.ReadUInt16();
        var bitmapMatrix = ReadMatrixRecord();

        return new FillStyle(type, bitmapId, bitmapMatrix);
    }

    /// <summary>
    /// Reads a single line style.
    /// </summary>
    /// <returns>The line style.</returns>
    internal LineStyle ReadLineStyle()
    {
        // TODO(pref): Read an RGBA record for Shape3.
        // TODO(pref): Support LineStyle2 (DefineShape4).
        return new LineStyle
        {
            Width = _reader.ReadUInt16(),
            Color = ReadRGBRecord()
        };
    }

    /// <summary>
    /// Reads the header of this swf file.
    /// </summary>
    /// <returns>The header.</returns>
    internal SwfHeader ReadHeader()
    {
        // According to the SWF spec, the version and file length are supposed to be read
        // before we decompress the file, making this a bit annoying to parse.
        var version = (byte)_baseStream.ReadByte();
        var fileLength = (uint)(_baseStream.ReadByte() | 
            _baseStream.ReadByte() <<  8 |
            _baseStream.ReadByte() << 16 |
            _baseStream.ReadByte() << 24);

        return new SwfHeader
        {
            Version = version,
            FileLength = fileLength,
            FrameSize = ReadRectangleRecord(),
            FrameRate = _reader.ReadUInt16(),
            FrameCount = _reader.ReadUInt16()
        };
    }

    /// <summary>
    /// Reads an swf tag.
    /// </summary>
    /// <returns>The tag.</returns>
    internal Tag ReadTag()
    {
        var record = ReadRecordHeader();
        Console.WriteLine($"Read tag {record.Type} of length {record.Length}");

        var tag = record.Type switch
        {
            TagType.SetBackgroundColor => new SetBackgroundColor().Parse(this, record),
            TagType.DefineBitsLossless => new DefineBitsLossless(version: 1).Parse(this, record),
            TagType.DefineBitsLossless2 => new DefineBitsLossless(version: 2).Parse(this, record),
            //TagType.DefineShape => new DefineShape().Parse(this, record),
            _ => null!
        };

        // If we haven't implemented this tag, skip the length.
        if (tag is null)
            _reader.ReadBytes(record.Length);

        return tag!;
    }

    /// <summary>
    /// Gets the underlying binary reader.
    /// </summary>
    /// <returns>The binary reader.</returns>
    internal BinaryReader GetBinaryReader()
    {
        return _reader;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _reader?.Dispose();
        _readerStream?.Dispose();

        GC.SuppressFinalize(this);
    }
}
