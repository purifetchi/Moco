﻿using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;
using Moco.Exceptions;
using Moco.SWF.Actions;
using Moco.SWF.Actions.SWF3;
using Moco.SWF.DataTypes;
using Moco.SWF.Serialization.Internal;
using Moco.SWF.Tags;
using Moco.SWF.Tags.Control;
using Moco.SWF.Tags.Definition;
using Moco.SWF.Tags.Definition.Shapes;
using Moco.SWF.Tags.Definition.Shapes.Records;

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
            SwfCompressionMode.Lzma => throw new MocoTodoException("Lzma compression not yet supported."),
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

        Tag tag;
        do
        {
            try
            {
                tag = ReadTag();
                swf.AddTag(tag);
            }
            catch (MocoTodoException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
                return swf;
            }

        } while (tag is not End);

        return swf;
    }

    /// <summary>
    /// Reads a 8.8 fixed point float number.
    /// </summary>
    /// <returns>The fixed point number.</returns>
    internal float Read88FixedPoint()
    {
        const int fractionalPartMask = 0xFF;
        const float fractionalDivisor = 1f / (byte.MaxValue + 1f);
        var value = _reader.ReadUInt16();

        var fractionalPart = value & (fractionalPartMask);
        var integerPart = (value & ~fractionalPartMask) >> 8;

        return integerPart + fractionalPart * fractionalDivisor;
    }

    /// <summary>
    /// Reads a null byte terminated string.
    /// </summary>
    /// <returns>The string.</returns>
    internal string ReadZeroTerminatedString()
    {
        var characterList = new List<byte>();
        while (true)
        {
            var character = _reader.ReadByte();
            if (character == 0)
                break;

            characterList.Add(character);
        }

        return Encoding.ASCII.GetString(characterList.ToArray());
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
    /// Reads a color transform with alpha.
    /// </summary>
    /// <returns>The color transform.</returns>
    internal ColorTransform ReadCXFormWithAlphaRecord()
    {
        const int flagCount = 2;
        const int nBitsLength = 4;

        var br = new BitReader(_reader);
        
        var flags = (ColorTransformMode)br.ReadUnsignedBits(flagCount);
        var nBits = br.ReadUnsignedBits(nBitsLength);

        var rAdd = 0;
        var gAdd = 0;
        var bAdd = 0;
        var aAdd = 0;
        if (flags.HasFlag(ColorTransformMode.Addition))
        {
            rAdd = br.ReadSignedBits(nBits);
            gAdd = br.ReadSignedBits(nBits);
            bAdd = br.ReadSignedBits(nBits);
            aAdd = br.ReadSignedBits(nBits);
        }

        var rMult = 0;
        var gMult = 0;
        var bMult = 0;
        var aMult = 0;
        if (flags.HasFlag(ColorTransformMode.Multiplication))
        {
            rMult = br.ReadSignedBits(nBits);
            gMult = br.ReadSignedBits(nBits);
            bMult = br.ReadSignedBits(nBits);
            aMult = br.ReadSignedBits(nBits);
        }

        return new ColorTransform
        {
            Mode = flags,

            RedAddTerm = rAdd,
            GreenAddTerm = gAdd,
            BlueAddTerm = bAdd,
            AlphaAddTerm = aAdd,

            RedMultTerm = rMult,
            GreenMultTerm = gMult,
            BlueMultTerm = bMult,
            AlphaMultTerm = aMult
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
    /// Reads a gradientrecord record.
    /// </summary>
    /// <param name="ctx">The style reading context.</param>
    /// <returns>The gradientrecord record.</returns>
    internal GradientRecord ReadGradientRecordRecord(StyleReadingContext ctx)
    {
        return new GradientRecord
        {
            Ratio = _reader.ReadByte(),
            Color = ctx.ReadRGBA ? ReadRGBARecord() : ReadRGBRecord()
        };
    }
    
    /// <summary>
    /// Reads a gradient record.
    /// </summary>
    /// <param name="ctx">The style reading context.</param>
    /// <returns>The gradient record.</returns>
    internal Gradient ReadGradientRecord(StyleReadingContext ctx)
    {
        const int enumBits = 2;
        const int numGradientsLength = 4;

        var br = new BitReader(_reader);
        var spread = br.ReadEnum<SpreadMode>(enumBits);
        var interpolation = br.ReadEnum<InterpolationMode>(enumBits);
        var nGrads = br.ReadUnsignedBits(numGradientsLength);

        var records = new GradientRecord[nGrads];
        for (var i = 0; i < nGrads; i++)
            records[i] = ReadGradientRecordRecord(ctx);

        return new Gradient(spread, interpolation, records);
    }

    /// <summary>
    /// Reads a single fill style.
    /// </summary>
    /// <param name="ctx">The style reading context.</param>
    /// <returns>The fill style.</returns>
    internal FillStyle ReadFillStyle(StyleReadingContext ctx)
    {
        var type = (FillStyleType)_reader.ReadByte();

        switch (type)
        {
            case FillStyleType.Solid:
                var color = ctx.ReadRGBA ? ReadRGBARecord() : ReadRGBRecord();
                return new FillStyle(type, color);

            case FillStyleType.RepeatingBitmap:
            case FillStyleType.ClippedBitmap:
            case FillStyleType.NonSmoothedRepeatingBitmap:
            case FillStyleType.NonSmoothedClippedBitmap:
                var bitmapId = _reader.ReadUInt16();
                var bitmapMatrix = ReadMatrixRecord();

                return new FillStyle(type, bitmapId, bitmapMatrix);

            case FillStyleType.LinearGradient:
            case FillStyleType.RadialGradient:
                var gradientMatrix = ReadMatrixRecord();
                var gradient = ReadGradientRecord(ctx);
                return new FillStyle(type, gradientMatrix, gradient);

            default:
                throw new MocoTodoException($"Fill type {type} is not yet supported.");
        }
    }

    /// <summary>
    /// Reads a single line style.
    /// </summary>
    /// <param name="ctx">The style reading context.</param>
    /// <returns>The line style.</returns>
    internal LineStyle ReadLineStyle(StyleReadingContext ctx)
    {
        if (ctx.ReadLineStyle2)
        {
            const int enumBitLength = 2;
            const int flagBitLength = 10;

            var width = _reader.ReadUInt16();
            var br = new BitReader(_reader);
            
            var startCap = br.ReadEnum<CapStyle>(enumBitLength);
            var joinStyle = br.ReadEnum<JoinStyle>(enumBitLength);

            var flags = br.ReadEnum<LineStyle2Flags>(flagBitLength);

            var endCap = br.ReadEnum<CapStyle>(enumBitLength);

            var miterLimitFactor = joinStyle == JoinStyle.Miter ? Read88FixedPoint() : 0f;
            var color = !flags.HasFlag(LineStyle2Flags.HasFill) ? ReadRGBARecord() : new Rgba();
            var fillType = flags.HasFlag(LineStyle2Flags.HasFill) ? ReadFillStyle(ctx) : null;

            return new LineStyle2
            {
                Width = width,
                StartCapStyle = startCap,
                JoinStyle = joinStyle,
                EndCapStyle = endCap,
                Flags = flags,
                MiterLimitFactor = miterLimitFactor,
                Color = color,
                FillType = fillType,
            };
        }

        return new LineStyle
        {
            Width = _reader.ReadUInt16(),
            Color = ctx.ReadRGBA ? ReadRGBARecord() : ReadRGBRecord()
        };
    }

    /// <summary>
    /// Reads a list of shape records.
    /// </summary>
    /// <param name="ctx">The reading context.</param>
    /// <returns>The shape record list.</returns>
    internal List<IShapeRecord> ReadShapeRecordsList(ShapeRecordReadingContext ctx)
    {
        var list = new List<IShapeRecord>();

        // Apparently the swf spec says that individual records are byte-boundary aligned
        // within the list, but that's a blatant lie. Any record can peacefully invade
        // another record's byte value and Flash Player will happily read and parse it.
        var br = new BitReader(_reader);

        IShapeRecord record;
        do
        {
            record = ReadShapeRecord(ref br, ref ctx);
            list.Add(record);
        } while (record is not EndShapeRecord);

        return list;
    }

    /// <summary>
    /// Reads a single shape record.
    /// </summary>
    /// <param name="br">The bit reader to read from.</param>
    /// <param name="ctx">The reading context.</param>
    /// <returns>The shape record.</returns>
    internal IShapeRecord ReadShapeRecord(
        ref BitReader br,
        ref ShapeRecordReadingContext ctx)
    {
        var type = (ShapeRecordType)br.ReadBit();
        //Console.WriteLine($"[ReadShapeRecord] Beginning to read record of type {type}");

        if (type == ShapeRecordType.NonEdgeRecord)
        {
            // Bits required to be read to see if this is an end record.
            const int bitsForEndMarker = 5;

            // If the end bits are all 0, then we're dealing with an end shape record
            // we can bail here.
            var flags = (StyleChangeRecordFlags)br.ReadUnsignedBits(bitsForEndMarker);
            if (flags == StyleChangeRecordFlags.IsEndMarker)
                return new EndShapeRecord();

            return new StyleChangeRecord(flags)
                .Parse(ref br, this, ref ctx);
        }
        else
        {
            // Decides whether this is a straight edge record or a curved one.
            var isStraight = br.ReadBitFlag();
            if (isStraight)
                return new StraightEdgeRecord().Parse(ref br);
            else
                return new CurvedEdgeRecord().Parse(ref br);
        }
    }

    /// <summary>
    /// Wrapper for reading (Fill/Line)Style arrays.
    /// </summary>
    /// <typeparam name="TStyle">The type we're reading.</typeparam>
    /// <param name="ctx">The style reading context.</param>
    /// <param name="reader">The method for reading.</param>
    /// <returns>The style array.</returns>
    internal TStyle[] ReadStyleArray<TStyle>(
        StyleReadingContext ctx, 
        Func<StyleReadingContext, TStyle> reader)
    {
        var styleCount = _reader.ReadByte();

        // TODO(pref): Support the extended count.
        if (styleCount == 0xFF)
            throw new MocoTodoException("[ReadStyleArray] Extended count not yet supported.");

        var styles = new TStyle[styleCount];
        for (var i = 0; i < styleCount; i++)
            styles[i] = reader(ctx);

        return styles;
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
            FrameRate = Read88FixedPoint(),
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
        //Console.WriteLine($"Read tag {record.Type} of length {record.Length}");

        Tag tag = record.Type switch
        {
            TagType.SetBackgroundColor => new SetBackgroundColor(),
            TagType.DefineBitsLossless => new DefineBitsLossless(version: 1),
            TagType.DefineBitsLossless2 => new DefineBitsLossless(version: 2),
            TagType.RemoveObject => new RemoveObject(version: 1),
            TagType.RemoveObject2 => new RemoveObject(version: 2),
            TagType.PlaceObject2 => new PlaceObject(version: 2),
            TagType.DefineShape => new DefineShape(version: 1),
            TagType.DefineShape2 => new DefineShape(version: 2),
            TagType.DefineShape3 => new DefineShape(version: 3),
            TagType.DefineShape4 => new DefineShape(version: 4),
            TagType.DefineSprite => new DefineSprite(),
            TagType.ShowFrame => new ShowFrame(),
            TagType.DoAction => new DoAction(),
            TagType.End => new End(),
            _ => null!
        };

        // If we haven't implemented this tag, skip the length.
        if (tag is null)
        {
            _reader.ReadBytes(record.Length);
            return tag!;
        }

        return tag!.Parse(this, record);
    }

    /// <summary>
    /// Reads an action.
    /// </summary>
    /// <returns>The action.</returns>
    public SwfAction? ReadAction()
    {
        const byte hasLengthBoundary = 0x80;

        // If the type is 0, we can exit. That's the end marker.
        var type = (ActionType)_reader.ReadByte();
        if (type == 0x00)
            return null;

        // If the tag is above 0x80, we need to read its length.
        var length = 0;
        if ((byte)type >= hasLengthBoundary)
            length = _reader.ReadUInt16();

        return type switch
        {
            ActionType.Play => new PlayAction().Parse(this),
            ActionType.Stop => new StopAction().Parse(this),
            ActionType.WaitForFrame => new WaitForFrameAction().Parse(this),
            ActionType.GotoFrame => new GoToFrameAction().Parse(this),
            _ => new DummyAction(type, length).Parse(this)
        };
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
