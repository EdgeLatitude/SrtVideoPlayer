using System;

namespace SrtVideoPlayer.Shared.Models.Playback
{
    public class Playback
    {
        public Video Video { get; set; }
        public TimeSpan Time { get; set; }
        public Subtitle[] Subtitles { get; set; }

        public Playback()
        {

        }

        public Playback(Video video, TimeSpan time, Subtitle[] subtitles)
        {
            Video = video;
            Time = time;
            Subtitles = subtitles;
        }
    }
}
