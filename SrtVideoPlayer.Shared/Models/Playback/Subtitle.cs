namespace SrtVideoPlayer.Shared.Models.Playback
{
    public class Subtitle
    {
        public int Index { get; set; }
        public SubtitleSpan SubtitleSpan { get; set; }
        public string Text { get; set; }

        public Subtitle()
        {

        }

        public Subtitle(int index, SubtitleSpan subtitleSpan, string text)
        {
            Index = index;
            SubtitleSpan = subtitleSpan;
            Text = text;
        }
    }
}
