using System;

namespace SrtVideoPlayer.Shared.Models.Playback
{
    public class SubtitleSpan
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public SubtitleSpan()
        {

        }

        public SubtitleSpan(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }
    }
}
