using System.Collections.ObjectModel;

namespace SrtVideoPlayer.Shared.Constants
{
    public static class HardwareInput
    {
        public static readonly ReadOnlyCollection<string> KeyboardShortcutsCollection = new ReadOnlyCollection<string>(new string[]
        {
            KeyboardShortcuts.PlayPauseA,
            KeyboardShortcuts.PlayPauseB,
            KeyboardShortcuts.GoBack10_Seconds,
            KeyboardShortcuts.GoForward10_Seconds,
            KeyboardShortcuts.Restart,
            KeyboardShortcuts.FullscreenOnOff,
            KeyboardShortcuts.MuteUnmute,
            KeyboardShortcuts.CaptionsOnOff
        });
    }
}
