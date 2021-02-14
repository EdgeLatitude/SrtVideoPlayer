using Android.Widget;
using SrtVideoPlayer.Mobile.Droid.Effects;
using SrtVideoPlayer.Mobile.Effects;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName(nameof(SrtVideoPlayer))]
[assembly: ExportEffect(typeof(LabelShadowEffect), nameof(ShadowEffect))]
namespace SrtVideoPlayer.Mobile.Droid.Effects
{
    public class LabelShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var control = Control as TextView;
                var effect = (ShadowEffect)Element.Effects.FirstOrDefault(innerEffect => innerEffect is ShadowEffect);
                if (effect == null)
                    return;
                var radius = effect.Radius;
                var distanceX = effect.DistanceX;
                var distanceY = effect.DistanceY;
                var color = effect.Color.ToAndroid();
                control.SetShadowLayer(radius, distanceX, distanceY, color);
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
