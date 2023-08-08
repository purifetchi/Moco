using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moco.SWF.Serialization;

/// <summary>
/// An exception thrown when the read file is not an swf.
/// </summary>
public class NotAnSwfException : Exception
{
    /// <summary>
    /// Constructs a new NotAnSwfException.
    /// </summary>
    public NotAnSwfException()
        : base("The stream does not contain a shockwave flash file.")
    {

    }

    /// <summary>
    /// Constructs a new NotAnSwfException for a file.
    /// </summary>
    /// <param name="filename">The file.</param>
    public NotAnSwfException(string filename)
        : base($"File {filename} is not a shockwave flash file!")
    {

    }
}
