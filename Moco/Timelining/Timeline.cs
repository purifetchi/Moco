using System.Diagnostics;
using Moco.Exceptions;
using Moco.Rendering;
using Moco.Rendering.Display;
using Moco.SWF.Tags;
using Moco.SWF.Tags.Control;

namespace Moco.Timelining;

/// <summary>
/// The timeline, containing frames and the display list.
/// </summary>
public class Timeline
{
    /// <summary>
    /// The display list.
    /// </summary>
    public DisplayList DisplayList { get; init; }

    /// <summary>
    /// The current frame index.
    /// </summary>
    public int FrameIndex { get; private set; } = -1;

    /// <summary>
    /// Gets or sets whether the timeline is running.
    /// </summary>
    public bool Active
    {
        get => _sw.IsRunning;
        set
        {
            if (value)
                _sw.Start();
            else
                _sw.Stop();
        }
    }

    /// <summary>
    /// The framerate.
    /// </summary>
    public float FrameRate { get; init; }

    /// <summary>
    /// The loop count.
    /// </summary>
    public int LoopCount { get; init; }

    /// <summary>
    /// The frame list.
    /// </summary>
    public IReadOnlyList<Frame> Frames => _frames;

    /// <summary>
    /// The frame list.
    /// </summary>
    private readonly List<Frame> _frames;

    /// <summary>
    /// The ticking stopwatch.
    /// </summary>
    private readonly Stopwatch _sw;

    /// <summary>
    /// The loops we've done so far.
    /// </summary>
    private int _loops = 0;

    /// <summary>
    /// Constructs a new timeline and parses the tags into frames.
    /// </summary>
    /// <param name="tags">The tag list.</param>
    /// <param name="frameRate">The framerate of the timeline.</param>
    /// <param name="loopCount">The loop count of the timeline.</param>
    public Timeline(
        IEnumerable<Tag> tags,
        float frameRate,
        int loopCount)
    {
        DisplayList = new();
        _frames = new();
        _sw = new();

        FrameRate = frameRate;
        LoopCount = loopCount;

        ParseTagsIntoFrames(tags);
    }

    /// <summary>
    /// Parses a tag enumerable into a list of frames.
    /// </summary>
    /// <param name="tags">The tag enumerable.</param>
    private void ParseTagsIntoFrames(IEnumerable<Tag> tags)
    {
        var frame = new Frame();
        foreach (var tag in tags)
        {
            if (tag is IControlTag controlTag)
            {
                frame.AddTag(controlTag);
                continue;
            }

            if (tag is ShowFrame)
            {
                _frames.Add(frame);
                frame = new();
            }
        }

        AdvanceFrame();
    }

    /// <summary>
    /// Advances the current frame.
    /// </summary>
    public void AdvanceFrame()
    {
        FrameIndex++;
        
        if (FrameIndex == _frames.Count)
        {
            // Restart if we still can
            if (_loops < LoopCount)
            {
                _loops++;
                FrameIndex = 0;
            }
            else
            {
                _sw.Reset();
                return;
            }
        }

        var frame = Frames[FrameIndex];
        foreach (var tag in frame.EffectorTags)
        {
            // TODO(pref): Move this code somewhere that makes sense.
            if (tag is PlaceObject placeObject)
            {
                if (placeObject.Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
                    !placeObject.Flags.HasFlag(PlaceObjectFlags.Move))
                {
                    DisplayList.Push(new Rendering.Display.Object
                    {
                        CharacterId = placeObject.CharacterId,
                        Depth = placeObject.Depth,
                        Matrix = placeObject.Matrix
                    });
                }

                if (placeObject.Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
                    placeObject.Flags.HasFlag(PlaceObjectFlags.Move))
                {
                    DisplayList.RemoveAtDepth(placeObject.Depth);
                    DisplayList.Push(new Rendering.Display.Object
                    {
                        CharacterId = placeObject.CharacterId,
                        Depth = placeObject.Depth,
                        Matrix = placeObject.Matrix
                    });
                }

                if (!placeObject.Flags.HasFlag(PlaceObjectFlags.HasCharacter) &&
                    placeObject.Flags.HasFlag(PlaceObjectFlags.Move))
                {
                    var maybeShape = DisplayList.GetAtDepth(placeObject.Depth);
                    if (maybeShape is Rendering.Display.Object shape)
                        shape.Matrix = placeObject.Matrix;
                }
            }
            else if (tag is RemoveObject removeObject)
            {
                if (removeObject.CharacterId.HasValue)
                    throw new MocoTodoException(TagType.RemoveObject, "Care about the character id when removing.");

                DisplayList.RemoveAtDepth(removeObject.Depth);
            }
        }

        _sw.Restart();
    }

    /// <summary>
    /// Ticks the timeline.
    /// </summary>
    public void Tick()
    {
        if (!_sw.IsRunning && _loops < LoopCount)
            _sw.Start();

        if (_sw.ElapsedMilliseconds / 1000f >= 1 / FrameRate)
            AdvanceFrame();
    }

    /// <summary>
    /// Draws the objects on the timeline.
    /// </summary>
    /// <param name="ctx">The drawing context.</param>
    public void Draw(DisplayListDrawingContext ctx)
    {
        if (!_sw.IsRunning)
            return;

        foreach (var item in DisplayList.Entries)
            item.Draw(ctx);
    }
}
