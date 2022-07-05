using AndroidX.Core.View;
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
            var window = Platform.CurrentActivity.Window;
            WindowCompat.SetDecorFitsSystemWindows(window, true);
            var controllerCompat = new WindowInsetsControllerCompat(window, window.DecorView);
            controllerCompat.Show(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.NavigationBars());
            controllerCompat.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowBarsBySwipe;
        });

        public void Enable(bool landscapeMode) => Device.BeginInvokeOnMainThread(() =>
        {
            Platform.CurrentActivity.RequestedOrientation = landscapeMode ?
                Android.Content.PM.ScreenOrientation.SensorLandscape :
                Android.Content.PM.ScreenOrientation.SensorPortrait;
            var window = Platform.CurrentActivity.Window;
            WindowCompat.SetDecorFitsSystemWindows(window, false);
            var controllerCompat = new WindowInsetsControllerCompat(window, window.DecorView);
            controllerCompat.Hide(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.NavigationBars());
            controllerCompat.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
        });
    }
}
