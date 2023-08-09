using Moco.Rendering;
using Moco.SWF;
using Moco.SWF.Serialization;
using Moco.SWF.Tags.Control;
using Moco.SWF.Tags.Definition;

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
    /// Constructs a new moco instance for the given backend.
    /// </summary>
    /// <param name="backend">The backend.</param>
    public MocoEngine(IMocoRendererBackend backend)
    {
        Backend = backend;
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

            Console.WriteLine($"Registering character of type {tag.Type} and id [{characterDefinitionTag.CharacterId}]");
        }
    }

    /// <summary>
    /// Loads an swf.
    /// </summary>
    /// <param name="filename">The swf.</param>
    public void LoadSwf(string filename)
    {
        using var reader = new SwfReader(filename);
        Swf = reader.ReadSwf();

        RegisterCharacters();
        PrepareWindow();
    }
}
