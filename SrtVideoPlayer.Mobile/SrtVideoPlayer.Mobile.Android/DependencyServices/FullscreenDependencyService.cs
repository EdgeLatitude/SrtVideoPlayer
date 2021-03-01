using Android.Views;
using SrtVideoPlayer.Mobile.DependencyServices;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SrtVideoPlayer.Mobile.Droid.DependencyServices.FullscreenDependencyService))]
namespace SrtVideoPlayer.Mobile.Droid.DependencyServices
{
    public class FullscreenDependencyService : IFullscreenDependencyService
    {
        public void Disable() => Device.BeginInvokeOnMainThread(() =>
        {
            Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Unspecified;
            Platform.CurrentActivity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LayoutStable;
        });

        public void Enable(bool landscapeMode) => Device.BeginInvokeOnMainThread(() =>
        {
            Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape;
            Platform.CurrentActivity.Window.DecorView.SystemUiVisibility =
                (StatusBarVisibility)(SystemUiFlags.Fullscreen
                    | SystemUiFlags.HideNavigation
                    | SystemUiFlags.Immersive
                    | SystemUiFlags.ImmersiveSticky
                    | SystemUiFlags.LowProfile
                    | SystemUiFlags.LayoutStable
                    | SystemUiFlags.LayoutHideNavigation
                    | SystemUiFlags.LayoutFullscreen);
        });
    }
}
