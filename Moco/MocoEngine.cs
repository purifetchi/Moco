using Moco.Rendering;
using Moco.SWF;
using Moco.SWF.Serialization;
using Moco.SWF.Tags.Control;

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
    /// Loads an swf.
    /// </summary>
    /// <param name="filename">The swf.</param>
    public void LoadSwf(string filename)
    {
        using var reader = new SwfReader(filename);
        Swf = reader.ReadSwf();

        PrepareWindow();
    }
}
