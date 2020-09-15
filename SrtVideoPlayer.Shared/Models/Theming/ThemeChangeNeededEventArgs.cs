using System;

namespace SrtVideoPlayer.Shared.Models.Theming
{
    public class ThemeChangeNeededEventArgs : EventArgs
    {
        public Theme Theme { get; set; }
    }
}
