using Android.Content.Res;
using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Shared.Models.Theming;
using Plugin.CurrentActivity;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SrtVideoPlayer.Mobile.Droid.DependencyServices.ThemingDependencyService))]
namespace SrtVideoPlayer.Mobile.Droid.DependencyServices
{
    public class ThemingDependencyService : IThemingDependencyService
    {
        public bool DeviceSupportsManualDarkMode() =>
            true;

        public bool DeviceSupportsAutomaticDarkMode() =>
            true;

        public bool DeviceRequiresPagesRedraw() =>
            true;

        public Theme GetDeviceDefaultTheme() =>
            Theme.Dark;

        public Task<Theme> GetDeviceTheme()
        {
            if (DeviceSupportsAutomaticDarkMode())
            {
                var uiModeFlags = CrossCurrentActivity.Current.AppContext.Resources.Configuration.UiMode
                    & UiMode.NightMask;
                switch (uiModeFlags)
                {
                    case UiMode.NightYes:
                        return Task.FromResult(Theme.Dark);
                    case UiMode.NightNo:
                        return Task.FromResult(Theme.Light);
                }
            }

            return Task.FromResult(GetDeviceDefaultTheme());
        }
    }
}
