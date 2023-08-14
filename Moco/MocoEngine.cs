using Moco.Rasterization;
using Moco.Rendering;
using Moco.SWF;
using Moco.SWF.Serialization;
using Moco.SWF.Tags.Control;
using Moco.SWF.Tags.Definition;
using Moco.Timelining;

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
    /// The main timeline.
    /// </summary>
    private Timeline? _timeline;

    /// <summary>
    /// Constructs a new moco instance for the given backend.
    /// </summary>
    /// <param name="backend">The backend.</param>
    public MocoEngine(IMocoRendererBackend backend)
    {
        Backend = backend;
        _objectDictionary = new();

        Backend.RenderFrameCallback = Tick;
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

                        new PathBuilder(defineShape.ShapeWithStyle!)
                            .CreateEdgeMaps()
                            .Rasterize(ctx);

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
    /// Runs the logic and then draws.
    /// </summary>
    private void Tick()
    {
        _timeline?.Tick();
        _timeline?.Draw(new DisplayListDrawingContext(this));
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
        _timeline = new(
            Swf.Tags,
            Swf.FrameRate,
            int.MaxValue);

        RegisterCharacters();
        PrepareWindow();
    }
}
