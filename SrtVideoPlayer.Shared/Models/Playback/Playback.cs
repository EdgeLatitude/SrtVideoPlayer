namespace SrtVideoPlayer.Shared.Models.Playback
{
    public class Playback
    {
        public Video Video { get; set; }
        public Subtitle[] Subtitles { get; set; }

        public Playback()
        {

        }

        public Playback(Video video, Subtitle[] subtitles)
        {
            Video = video;
            Subtitles = subtitles;
        }
    }
}
