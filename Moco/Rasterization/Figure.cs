using Moco.SWF.DataTypes;
using Moco.SWF.Tags.Definition.Shapes;

namespace Moco.Rasterization;

/// <summary>
/// A single figure within a shape.
/// </summary>
internal class Figure
{
    /// <summary>
    /// The paths for filling.
    /// </summary>
    public Dictionary<int, EdgePath> FillPaths { get; init; }

    /// <summary>
    /// The paths for lines.
    /// </summary>
    public Dictionary<int, EdgePath> LinePaths { get; init; }

    /// <summary>
    /// The fill styles.
    /// </summary>
    public FillStyle[] FillStyles { get; set; }

    /// <summary>
    /// The line styles.
    /// </summary>
    public LineStyle[] LineStyles { get; set; }

    /// <summary>
    /// The coordinate map.
    /// </summary>
    private Dictionary<Point, List<DrawCommand>>? _coordMap;

    /// <summary>
    /// The reverse coordinate map.
    /// </summary>
    private Dictionary<Point, List<DrawCommand>>? _revCoordMap;

    /// <summary>
    /// Creates a new figure.
    /// </summary>
    public Figure(
        FillStyle[] fillStyles,
        LineStyle[] lineStyles)
    {
        FillPaths = new();
        LinePaths = new();

        FillStyles = fillStyles;
        LineStyles = lineStyles;
    }

    /// <summary>
    /// Adds a path to this figure.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="ctx">The styles context.</param>
    public void AddPath(EdgePath path, ShapeStylesContext ctx)
    {
        if (ctx.FillStyle0.HasValue)
        {
            if (!FillPaths.ContainsKey(ctx.FillStyle0.Value))
                FillPaths.Add(ctx.FillStyle0.Value, new());

            // Reverse the path, effectively turning the left fill into a right fill
            // and then add it into the fill path.
            // ...that is if i understood it correctly from the as3swf blog post.
            FillPaths[ctx.FillStyle0.Value].Merge(
                path.Reversed(ctx.FillStyle0.Value)
            );
        }

        if (ctx.FillStyle1.HasValue)
        {
            if (!FillPaths.ContainsKey(ctx.FillStyle1.Value))
                FillPaths.Add(ctx.FillStyle1.Value, new());

            // Treat the right-fill as the proper fill and add the edges verbatim
            // Which fill we care about is completely arbitrary, but most seem
            // to go with the right fill, so I will too.
            FillPaths[ctx.FillStyle1.Value].Merge(path);
        }

        if (ctx.LineStyle.HasValue)
        {
            if (!LinePaths.ContainsKey(ctx.LineStyle.Value))
                LinePaths.Add(ctx.LineStyle.Value, new());

            // An edge can only have one line style, we're good.
            LinePaths[ctx.LineStyle.Value].Merge(path);
        }
    }

    /// <summary>
    /// Cleans and reassembles the paths of this figure.
    /// </summary>
    public void Clean()
    {
        CleanEdgeMap(FillPaths);
        CleanEdgeMap(LinePaths);

        _coordMap?.Clear();
        _coordMap = null!;

        _revCoordMap?.Clear();
        _revCoordMap = null!;
    }

    /// <summary>
    /// Flattens this figure's fills.
    /// </summary>
    public EdgePath FlattenFills()
    {
        return new EdgePath(
            FillPaths.Keys.OrderBy(i => i)
            .SelectMany(i => FillPaths[i].Commands));
    }

    /// <summary>
    /// Flattens this figure's lines.
    /// </summary>
    public EdgePath FlattenLines()
    {
        return new EdgePath(
            LinePaths.Keys.OrderBy(i => i)
            .SelectMany(i => LinePaths[i].Commands));
    }


    /// <summary>
    /// Cleans a single edge map, that is, if I understand correctly, it joins all of the disjoint edges/commands
    /// in this figure and slims it down.
    /// <br />
    /// I take absolutely no credit for this code, I'm way too stupid to comprehend most of it. Credit where
    /// credit is due, it came from as3swf.
    /// </summary>
    /// <param name="edgeMap">The map of the edges.</param>
    private void CleanEdgeMap(Dictionary<int, EdgePath> edgeMap)
    {
        foreach (var (styleIdx, subPath) in edgeMap)
        {
            if (subPath.Commands.Count == 0)
                continue;

            DrawCommand? lastCommand = null;
            var tmpPath = new EdgePath();

            // Create lookup maps for the path.
            CreateCoordMap(subPath);
            CreateReverseCoordMap(subPath);

            while (subPath.Commands.Count > 0)
            {
                var idx = 0;
                while (idx < subPath.Commands.Count)
                {
                    if (lastCommand is not null)
                    {
                        // If the previous command's To field isn't the next command's from field
                        // we need to find a command that'll fit.
                        if (lastCommand.To != subPath.Commands[idx].From)
                        {
                            // Find the next command in the coordinate map.
                            var nextCommand = FindNextCommandInCoordMap(lastCommand);

                            // If we've found it, we're good. Just get the index and go into the next iteration.
                            // This will set the "idx" field which signifies the next command.
                            if (nextCommand is not null)
                            {
                                idx = subPath.Commands.IndexOf(nextCommand);
                            }
                            else
                            {
                                // Otherwise try to find the command that should come before it.
                                var revEdge = FindNextCommandInReverseCoordMap(lastCommand);

                                if (revEdge is not null)
                                {
                                    // If we've found it, reverse that command, swapping the to and from values.
                                    // Basically it turns a set of commands that go like this -A-><-B- to ones that
                                    // go like this -A->-B->
                                    idx = subPath.Commands.IndexOf(revEdge);
                                    var r = revEdge.Reverse(revEdge.FillStyleIdx);
                                    UpdateCommandInCoordMap(revEdge, r);
                                    UpdateCommandInReverseCoordMap(revEdge, r);
                                    subPath.Commands[idx] = r;
                                }
                                else
                                {
                                    idx = 0;
                                    lastCommand = null;
                                }
                            }

                            continue;
                        }
                    }

                    // Remove the current command from the previous path and add it to the temporary
                    // path we've made. This will effectively reorder the path in a way that no segment
                    // is disjoint from its From field. At least this is what the code above should've
                    // done.
                    var currentCommand = subPath.Commands[idx];
                    subPath.Commands.RemoveAt(idx);
                    tmpPath.AddCommand(currentCommand);
                    RemoveCommandFromCoordMap(currentCommand);
                    RemoveCommandFromReverseCoordMap(currentCommand);
                    lastCommand = currentCommand;
                }
            }

            // Finally, replace the last path with the new one.
            edgeMap[styleIdx] = tmpPath;
        }
    }

    /// <summary>
    /// Finds the next command in the coordinate map relative to some other command.
    /// </summary>
    /// <param name="edge">The command.</param>
    /// <returns>The command that should come after this command.</returns>
    private DrawCommand? FindNextCommandInCoordMap(DrawCommand edge)
    {
        var key = edge.To;
        if (!_coordMap!.TryGetValue(key, out var path))
            return null;

        if (path.Count == 0)
            return null;

        return path[0];
    }

    /// <summary>
    /// Finds the previous command in the reverse coordinate map relative to some other command.
    /// </summary>
    /// <param name="edge">The command.</param>
    /// <returns>The command that should come before this command.</returns>
    private DrawCommand? FindNextCommandInReverseCoordMap(DrawCommand edge)
    {
        var key = edge.To;
        if (!_revCoordMap!.TryGetValue(key, out var path))
            return null;

        if (path.Count == 0)
            return null;

        return path[0];
    }

    /// <summary>
    /// Removes a command from the coordinate map.
    /// </summary>
    /// <param name="edge">The edge.</param>
    private void RemoveCommandFromCoordMap(DrawCommand edge)
    {
        var key = edge.From;
        if (_coordMap!.ContainsKey(key))
        {
            if (_coordMap[key].Count == 1)
                _coordMap.Remove(key);
            else
                _coordMap[key].Remove(edge);
        }
    }

    /// <summary>
    /// Removes a command from the reverse coordinate map.
    /// </summary>
    /// <param name="edge">The command.</param>
    private void RemoveCommandFromReverseCoordMap(DrawCommand edge)
    {
        var key = edge.To;
        if (_revCoordMap!.ContainsKey(key))
        {
            if (_revCoordMap[key].Count == 1) 
                _revCoordMap.Remove(key);
            else 
                _revCoordMap[key].Remove(edge);
        }
    }

    /// <summary>
    /// Creates a coordinate map, which is a map of commands that begin at a certain From point.
    /// </summary>
    /// <param name="path">The path to create it from.</param>
    private void CreateCoordMap(EdgePath path)
    {
        _coordMap ??= new();
        _coordMap.Clear();

        for (int i = 0; i < path.Commands.Count; i++)
        {
            var command = path.Commands[i];

            var key = command.From;
            if (!_coordMap.ContainsKey(key))
                _coordMap[key] = new();

            _coordMap[key].Add(command);
        }
    }

    /// <summary>
    /// Creates a reverse coordinate map, which is a map of commands that end at a certain To point.
    /// </summary>
    /// <param name="path">The path to create it from.</param>
    private void CreateReverseCoordMap(EdgePath path)
    {
        _revCoordMap ??= new();
        _revCoordMap.Clear();

        for (int i = 0; i < path.Commands.Count; i++)
        {
            var command = path.Commands[i];

            var key = command.To;

            if (!_revCoordMap.ContainsKey(key))
                _revCoordMap[key] = new();

            _revCoordMap[key].Add(command);
        }
    }

    /// <summary>
    /// Updates a command in the coordinate map, effectively swapping its from and to values.
    /// </summary>
    /// <param name="edge">The old command.</param>
    /// <param name="newEdge">The command that's replacing it.</param>
    private void UpdateCommandInCoordMap(
        DrawCommand edge, 
        DrawCommand newEdge)
    {
        var oldKey = edge.From;
        _coordMap![oldKey].Remove(edge);

        var newKey = newEdge.From;
        if (!_coordMap.ContainsKey(newKey))
            _coordMap[newKey] = new();

        _coordMap[newKey].Add(newEdge);
    }

    /// <summary>
    /// Updates a command in the reverse coordinate map, effectively swapping its to and from values.
    /// </summary>
    /// <param name="edge">The old command.</param>
    /// <param name="newEdge">The command that's replacing it.</param>
    private void UpdateCommandInReverseCoordMap(
        DrawCommand edge, 
        DrawCommand newEdge)
    {
        var oldKey = edge.To;
        _revCoordMap![oldKey].Remove(edge);

        var newKey = newEdge.To;
        if (!_revCoordMap.ContainsKey(newKey))
            _revCoordMap[newKey] = new();

        _revCoordMap[newKey].Add(newEdge);
    }
}
