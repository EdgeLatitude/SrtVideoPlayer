using System;
using System.Collections.Generic;

namespace SrtVideoPlayer.Shared.Models.Playback
{
    public class Playback
    {
        public Video Video { get; set; }
        public TimeSpan Time { get; set; }
        public Dictionary<TimeSpan, string> Subtitles { get; set; }

        public Playback()
        {

        }

        public Playback(Video video, TimeSpan time, Dictionary<TimeSpan, string> subtitles)
        {
            Video = video;
            Time = time;
            Subtitles = subtitles;
        }
    }
}
