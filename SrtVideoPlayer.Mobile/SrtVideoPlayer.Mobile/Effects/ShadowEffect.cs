using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.Effects
{
    public class ShadowEffect : RoutingEffect
    {
        public Color Color { get; set; } = Color.Black;

        public float DistanceX { get; set; } = 5;

        public float DistanceY { get; set; } = 5;

        public float Radius { get; set; } = 5;

        public ShadowEffect() : base($"{nameof(SrtVideoPlayer)}.{nameof(ShadowEffect)}")
        {
        }
    }
}
