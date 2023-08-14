namespace Moco.Rasterization;

/// <summary>
/// An edge path.
/// </summary>
internal class EdgePath
{
    /// <summary>
    /// The edge drawing commands.
    /// </summary>
    public IList<DrawCommand> Commands => _commands;

    /// <summary>
    /// The list of drawing commands.
    /// </summary>
    private List<DrawCommand> _commands;

    /// <summary>
    /// Constructs a new edge path.
    /// </summary>
    public EdgePath()
    {
        _commands = new();
    }

    /// <summary>
    /// Constructs a new edge path from a given set of commands.
    /// </summary>
    /// <param name="commands">The commands.</param>
    public EdgePath(IEnumerable<DrawCommand> commands)
    {
        _commands = new(commands);
    }

    /// <summary>
    /// Adds a new draw command.
    /// </summary>
    /// <param name="command">The draw command.</param>
    public void AddCommand(DrawCommand command)
    {
        _commands.Add(command);
    }

    /// <summary>
    /// Merges the commands of this and a different path together.
    /// </summary>
    /// <param name="commands">The commands.</param>
    public void Merge(IEnumerable<DrawCommand> commands)
    {
        _commands.AddRange(commands);
    }

    /// <summary>
    /// Merges the commands of this and a different path together.
    /// </summary>
    /// <param name="commands">The commands.</param>
    public void Merge(EdgePath commands)
    {
        _commands.AddRange(commands.Commands);
    }

    /// <summary>
    /// Reverses this path with a new style.
    /// </summary>
    /// <param name="fillStyle">The style.</param>
    public IEnumerable<DrawCommand> Reversed(int fillStyle)
    {
        return _commands.Select(edge => edge.Reverse(fillStyle))
            .Reverse();
    }
}
