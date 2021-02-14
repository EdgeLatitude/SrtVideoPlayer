using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.Effects
{
    public class ShadowEffect : RoutingEffect
    {
        public Color Color { get; set; }

        public float DistanceX { get; set; }

        public float DistanceY { get; set; }

        public float Radius { get; set; }

        public ShadowEffect() : base($"{nameof(SrtVideoPlayer)}.{nameof(ShadowEffect)}")
        {
        }
    }
}
