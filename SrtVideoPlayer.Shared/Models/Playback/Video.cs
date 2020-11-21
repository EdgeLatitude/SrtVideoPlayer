namespace SrtVideoPlayer.Shared.Models.Playback
{
    public class Video
    {
        public string Name { get; set; }
        public string Location { get; set; }

        public Video()
        {

        }

        public Video(string name, string location)
        {
            Name = name;
            Location = location;
        }
    }
}
