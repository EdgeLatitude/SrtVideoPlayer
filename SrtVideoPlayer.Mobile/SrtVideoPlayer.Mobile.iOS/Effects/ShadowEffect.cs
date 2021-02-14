using CoreGraphics;
using SrtVideoPlayer.Mobile.Effects;
using SrtVideoPlayer.Mobile.iOS.Effects;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName(nameof(SrtVideoPlayer))]
[assembly: ExportEffect(typeof(LabelShadowEffect), nameof(ShadowEffect))]
namespace SrtVideoPlayer.Mobile.iOS.Effects
{
    public class LabelShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var effect = (ShadowEffect)Element.Effects.FirstOrDefault(innerEffect => innerEffect is ShadowEffect);
                if (effect == null)
                    return;
                Control.Layer.ShadowRadius = effect.Radius;
                Control.Layer.ShadowColor = effect.Color.ToCGColor();
                Control.Layer.ShadowOffset = new CGSize(effect.DistanceX, effect.DistanceY);
                Control.Layer.ShadowOpacity = 1.0f;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Cannot set property on attached control. Error: {exception.Message}");
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
