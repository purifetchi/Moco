using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Moco.SWF.Serialization.Internal;

/// <summary>
/// A reader for bit structures.
/// </summary>
internal ref struct BitReader
{
    /// <summary>
    /// The underlying binary reader.
    /// </summary>
    private readonly BinaryReader _reader;

    /// <summary>
    /// The last read byte.
    /// </summary>
    private byte _lastByte = 0x00;

    /// <summary>
    /// The position.
    /// </summary>
    private sbyte _position = 7;

    /// <summary>
    /// Constructs a new bit reader from a binary reader.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    public BitReader(BinaryReader reader)
    {
        _reader = reader;
    }

    /// <summary>
    /// Ensures that we have stuff to read.
    /// </summary>
    private void EnsureCapacity()
    {
        const int bitsInByte = 8;

        if (_position >= bitsInByte)
        {
            _lastByte = _reader.ReadByte();
            _position = 0;
        }
    }

    /// <summary>
    /// Reads a single bit.
    /// </summary>
    /// <returns>The bit.</returns>
    public byte ReadBit()
    {
        const int offsetByteSize = (sizeof(byte) * 8) - 1;

        _position++;
        EnsureCapacity();
        
        var bit = (byte)(_lastByte & (1 << offsetByteSize - _position)) != 0 ? 
            (byte)0x1 : 
            (byte)0x0;
        return bit;
    }

    /// <summary>
    /// Reads a bit as a flag.
    /// </summary>
    /// <returns>The flag.</returns>
    public bool ReadBitFlag()
    {
        return ReadBit() != 0;
    }

    /// <summary>
    /// Reads an enum from the bit reader.
    /// </summary>
    /// <typeparam name="TEnum">The enum.</typeparam>
    /// <param name="bits">The length.</param>
    /// <returns>The enum.</returns>
    public TEnum ReadEnum<TEnum>(uint bits)
        where TEnum : struct, Enum
    {
        var value = ReadUnsignedBits(bits);
        return Unsafe.As<uint, TEnum>(ref value);
    }

    /// <summary>
    /// Reads a given amount of bits as a signed integer.
    /// </summary>
    /// <param name="bits">The amount of bits.</param>
    /// <returns>The resulting signed integer.</returns>
    public int ReadSignedBits(uint bits)
    {
        int value = (int)ReadUnsignedBits(bits);

        // When a signed-bit value is expanded into a larger word size, 
        // the high bit is copied to the leftmost bits.
        if ((value & (1 << (int)(bits - 1))) != 0)
        {
            var mask = (1 << (int)bits) - 1;
            value |= ~mask;
        }

        return value;
    }

    /// <summary>
    /// Reads a given amount of bits as an unsigned integer.
    /// </summary>
    /// <param name="bits">The amount of bits.</param>
    /// <returns>The resulting unsigned integer.</returns>
    public uint ReadUnsignedBits(uint bits)
    {
        // We always subtract an offset of 1 when setting the bits.
        const int offset = 1;

        var output = 0u;
        for (var i = 0; i < bits; i++)
            output |= ((uint)ReadBit()) << (int)(bits - offset - i);

        return output;
    }

    /// <summary>
    /// Reads a fixed point 16.16 floating bit value.
    /// </summary>
    /// <param name="bits">The amount of bits total.</param>
    /// <returns>The float value.</returns>
    public float ReadFloatingBits(uint bits)
    {
        const int fractionalPartMask = 0xFFFF;
        const float fractionalDivisor = 1f / (ushort.MaxValue + 1f);

        var value = ReadSignedBits(bits);

        // Split the value into two.
        var fractionalPart = value & (fractionalPartMask);
        var integerPart = (value & ~fractionalPartMask) >> 16;

        return integerPart + (fractionalPart * fractionalDivisor);
    }

    /// <summary>
    /// Rewinds a select amount of bits.
    /// </summary>
    /// <param name="bits">The amount of bits.</param>
    public void Rewind(sbyte bits)
    {
        _position -= bits;
    }
}
