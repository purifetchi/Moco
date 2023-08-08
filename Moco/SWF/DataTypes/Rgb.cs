using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moco.SWF.DataTypes;

/// <summary>
/// The RGB record represents a color as a 24-bit red, green, and blue value.
/// </summary>
public struct Rgb
{
    /// <summary>
    /// The red channel.
    /// </summary>
    public byte Red { get; set; }

    /// <summary>
    /// The green channel.
    /// </summary>
    public byte Green { get; set; }

    /// <summary>
    /// The blue channel.
    /// </summary>
    public byte Blue { get; set; }
}
