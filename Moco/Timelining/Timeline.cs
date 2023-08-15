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
            if (_sw.IsRunning == value)
                return;

            _paused = !value;

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
    /// Is the timeline paused?
    /// </summary>
    private bool _paused = false;

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
        var frame = new Frame
        {
            ClearsDisplayList = true
        };

        foreach (var tag in tags)
        {
            if (tag is IControlTag controlTag)
            {
                if (tag is DoAction doAction)
                {
                    foreach (var action in doAction.Actions!)
                        frame.AddAction(action);

                    continue;
                }

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
        var newIndex = FrameIndex + 1;
        
        if (newIndex == _frames.Count)
        {
            // Restart if we still can
            if (_loops < LoopCount)
            {
                _loops++;
                newIndex = 0;
            }
            else
            {
                _sw.Reset();
                return;
            }
        }

        SetFrame(newIndex);
    }

    /// <summary>
    /// Sets the frame by its index.
    /// </summary>
    /// <param name="index">The frame index.</param>
    public void SetFrame(int index)
    {
        if (index >= Frames.Count || index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Frame index isn't in the bounds [0; count]");

        // If the index isn't the next frame, we need to simulate up until that point.
        // TODO(pref): We can optimize it if the index is after the frame index, we
        //             only need to simulate the difference in frames.
        if (index != FrameIndex + 1)
        {
            for (var i = 0; i < index; i++)
            {
                var frame = Frames[i];
                frame.ExecuteTags(this);
            }
        }

        FrameIndex = index;

        var actualFrame = Frames[index];
        actualFrame.ExecuteTags(this);
        actualFrame.RunActions(new SWF.Actions.ActionExecutionContext(this));

        _sw.Restart();
    }

    /// <summary>
    /// Ticks the timeline.
    /// </summary>
    public void Tick()
    {
        if (_paused)
            return;

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
