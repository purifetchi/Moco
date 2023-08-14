namespace Moco.SWF.Actions;

/// <summary>
/// Action types.
/// </summary>
public enum ActionType : byte
{
    NextFrame = 0x04,
    PreviousFrame = 0x05,
    Play = 0x06,
    Stop = 0x07,
    ToggleQuality = 0x08,
    StopSounds = 0x09,

    GotoFrame = 0x81,
    GetURL = 0x83,
    WaitForFrame = 0x8A,
    SetTarget = 0x8B,
    GotoLabel = 0x8C
}
