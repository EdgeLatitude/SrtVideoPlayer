using System;
using System.Collections.Generic;

namespace SrtVideoPlayer.Shared.Models.Playback
{
    public class History
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public TimeSpan Time { get; set; }
        public Dictionary<TimeSpan, string> Subtitles { get; set; }

        public History()
        {

        }

        public History(string name, string location, TimeSpan time, Dictionary<TimeSpan, string> subtitles)
        {
            Name = name;
            Location = location;
            Time = time;
            Subtitles = subtitles;
        }
    }
}
