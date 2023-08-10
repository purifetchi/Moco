using System.Diagnostics;
using Moco.Rendering;
using Moco.Rendering.Display;
using Moco.SWF;
using Moco.SWF.Serialization;
using Moco.SWF.Tags;
using Moco.SWF.Tags.Control;
using Moco.SWF.Tags.Definition;
using Moco.SWF.Tags.Definition.Shapes.Records;

namespace Moco;

/// <summary>
/// The Moco flash emulator.
/// </summary>
public class MocoEngine
{
    /// <summary>
    /// The renderer backend.
    /// </summary>
    public IMocoRendererBackend Backend { get; init; }

    /// <summary>
    /// The current swf.
    /// </summary>
    public Swf? Swf { get; private set; }

    /// <summary>
    /// The object dictionary.
    /// </summary>
    private Dictionary<ushort, object> _objectDictionary;

    /// <summary>
    /// The frame display list.
    /// </summary>
    private DisplayList _displayList;

    /// <summary>
    /// The tag program counter.
    /// </summary>
    private int _tagPC = 0;

    /// <summary>
    /// The stopwatch.
    /// </summary>
    private Stopwatch _sw;

    /// <summary>
    /// Constructs a new moco instance for the given backend.
    /// </summary>
    /// <param name="backend">The backend.</param>
    public MocoEngine(IMocoRendererBackend backend)
    {
        Backend = backend;
        _objectDictionary = new();
        _displayList = new();
        _sw = new();

        Backend.RenderFrameCallback = DrawFrame;
    }

    /// <summary>
    /// Prepares the window for swf playback.
    /// </summary>
    private void PrepareWindow()
    {
        if (Swf is null)
            return;

        Backend?.SetWindowSize(Swf.FrameSize);

        var bgTag = Swf.GetTag<SetBackgroundColor>();
        if (bgTag is not null)
            Backend?.SetBackgroundClearColor(bgTag.BackgroundColor);
    }

    /// <summary>
    /// Registers all of the characters.
    /// </summary>
    private void RegisterCharacters()
    {
        if (Swf is null)
            return;

        foreach (var tag in Swf.Tags)
        {
            if (tag is not ICharacterDefinitionTag characterDefinitionTag)
                continue;

            switch (tag)
            {
                // TODO(pref): Move this code to a more sane location.
                case DefineShape defineShape:
                    {
                        var shape = Backend.RegisterShape(defineShape.CharacterId, defineShape.ShapeBounds);
                        using var ctx = shape.GetRasterizationContext();
                        ctx.SetEngine(this);
                        foreach (var record in defineShape.ShapeWithStyle!.ShapeRecords)
                        {
                            switch (record)
                            {
                                case StyleChangeRecord scr:
                                    if (scr.Flags.HasFlag(StyleChangeRecordFlags.HasMoveTo))
                                        ctx.MoveTo(scr.MoveDeltaX, scr.MoveDeltaY);

                                    if (scr.Flags.HasFlag(StyleChangeRecordFlags.HasFillStyle0))
                                    {
                                        if (scr.FillStyle0 == 0)
                                            ctx.SetFill(null!);
                                        else
                                            ctx.SetFill(defineShape.ShapeWithStyle!.FillStyles[scr.FillStyle0 - 1]);
                                    }

                                    if (scr.Flags.HasFlag(StyleChangeRecordFlags.HasFillStyle1))
                                    {
                                        if (scr.FillStyle1 == 0)
                                            ctx.SetFill(null!);
                                        else
                                            ctx.SetFill(defineShape.ShapeWithStyle!.FillStyles[scr.FillStyle1 - 1]);
                                    }
                                    break;

                                case StraightEdgeRecord ser:
                                    ctx.LineToRelative(ser.DeltaX, ser.DeltaY);
                                    break;

                                case CurvedEdgeRecord cer:
                                    ctx.CubicToRelative(cer.ControlDeltaX, cer.ControlDeltaY, cer.AnchorDeltaX, cer.AnchorDeltaY);
                                    break;
                            }
                        }
                        ctx.FlushPoints();

                        _objectDictionary.Add(defineShape.CharacterId, shape);
                    }
                    break;

                case DefineBitsLossless defineBitsLossless:
                    var imageObj = Backend.RegisterImageBytes(
                        defineBitsLossless.CharacterId,
                        defineBitsLossless.BitmapWidth,
                        defineBitsLossless.BitmapHeight,
                        defineBitsLossless.Pixmap!);

                    _objectDictionary.Add(defineBitsLossless.CharacterId, imageObj);
                    break;

                default:
                    Console.WriteLine($"Deal with {tag.Type}");
                    break;
            }

            Console.WriteLine($"Registering character of type {tag.Type} and id [{characterDefinitionTag.CharacterId}]");
        }
    }

    /// <summary>
    /// Prepares a frame.
    /// </summary>
    private void PrepareFrame()
    {
        Tag tag;
        do
        {
            tag = Swf!.Tags[_tagPC];

            // TODO(pref): Move this code somewhere that makes sense.
            if (tag is PlaceObject placeObject)
            {
                if (placeObject.Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
                    !placeObject.Flags.HasFlag(PlaceObjectFlags.Move))
                {
                    _displayList.Push(new Rendering.Display.Object
                    {
                        CharacterId = placeObject.CharacterId,
                        Depth = placeObject.Depth,
                        Matrix = placeObject.Matrix
                    });
                }

                if (placeObject.Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
                    placeObject.Flags.HasFlag(PlaceObjectFlags.Move))
                {
                    _displayList.RemoveAtDepth(placeObject.Depth);
                    _displayList.Push(new Rendering.Display.Object
                    {
                        CharacterId = placeObject.CharacterId,
                        Depth = placeObject.Depth,
                        Matrix = placeObject.Matrix
                    });
                }

                if (!placeObject.Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
                    placeObject.Flags.HasFlag(PlaceObjectFlags.Move))
                {
                    var maybeShape = _displayList.GetAtDepth(placeObject.Depth);
                    if (maybeShape is Rendering.Display.Object shape)
                        shape.Matrix = placeObject.Matrix;
                }
            }
            _tagPC++;

            // TODO(pref): Support limited loops.
            if (tag is End)
            {
                _displayList.Clear();
                _tagPC = 0;
            }
        } while (tag is not ShowFrame);

        _sw.Restart();
    }

    /// <summary>
    /// Draws a single frame.
    /// </summary>
    private void DrawFrame()
    {
        if (_sw.ElapsedMilliseconds / 1000f >= 1 / Swf!.FrameRate)
            PrepareFrame();

        var ctx = new DisplayListDrawingContext(this);
        foreach (var item in _displayList.Entries)
            item.Draw(ctx);
    }

    /// <summary>
    /// Gets a character from the dictionary
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>The character.</returns>
    public object? GetCharacter(ushort id)
    {
        if (!_objectDictionary.TryGetValue(id, out var result))
            return null;

        return result;
    }

    /// <summary>
    /// Loads an swf.
    /// </summary>
    /// <param name="filename">The swf.</param>
    public void LoadSwf(string filename)
    {
        using var reader = new SwfReader(filename);
        Swf = reader.ReadSwf();

        _sw.Restart();

        RegisterCharacters();
        PrepareWindow();
        PrepareFrame();
    }
}
