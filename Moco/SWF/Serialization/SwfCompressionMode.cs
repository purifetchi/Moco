using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moco.SWF.Serialization;

/// <summary>
/// The compression mode of this .swf file.
/// </summary>
public enum SwfCompressionMode
{
    /// <summary>
    /// No compression (magic: FWS).
    /// </summary>
    None = 0x535746,

    /// <summary>
    /// Zlib compression (magic: CWS).
    /// </summary>
    Zlib = 0x535743,

    /// <summary>
    /// LZMA compression (magic: ZWS).
    /// </summary>
    Lzma = 0x53575A
}
