﻿using System.IO.Compression;
using System.Reflection.PortableExecutable;
using Moco.SWF.DataTypes;
using Moco.SWF.Serialization.Internal;
using Moco.SWF.Tags;

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

        var header = ReadHeader();
        while (true)
        {
            try
            {
                ReadTag();
            }
            catch
            {
                break;
            }
        }
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

        // TODO(pref): Actually read the tag.
        _reader.ReadBytes(record.Length);

        return new Tag();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _reader?.Dispose();
        _readerStream?.Dispose();
    }
}