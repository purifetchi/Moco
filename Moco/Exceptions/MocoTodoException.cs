using Moco.SWF.Tags;

namespace Moco.Exceptions;

/// <summary>
/// A todo.
/// </summary>
public class MocoTodoException : Exception
{
    /// <summary>
    /// Constructs a new TODO exception for a tag.
    /// </summary>
    /// <param name="type">The tag.</param>
    /// <param name="message">The todo message.</param>
    public MocoTodoException(TagType type, string message)
        : base($"[{type}] TODO: {message}")
    {

    }

    /// <summary>
    /// Constructs a new TODO exception.
    /// </summary>
    /// <param name="message">The todo message.</param>
    public MocoTodoException(string message)
        : base($"TODO: {message}")
    {

    }
}
