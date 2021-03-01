using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Shared.Models.Theming;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SrtVideoPlayer.Mobile.iOS.DependencyServices.ThemingDependencyService))]
namespace SrtVideoPlayer.Mobile.iOS.DependencyServices
{
    public class ThemingDependencyService : IThemingDependencyService
    {
        public bool DeviceSupportsManualDarkMode() =>
            false;

        public bool DeviceSupportsAutomaticDarkMode() =>
            // Ensure the current device is running 12.0 or higher, 
            // because TraitCollection.UserInterfaceStyle was introduced in iOS 12.0
            UIDevice.CurrentDevice.CheckSystemVersion(12, 0);

        public bool DeviceRequiresPagesRedraw() =>
            false;

        public Theme GetDeviceDefaultTheme() =>
            Theme.Light;

        public async Task<Theme> GetDeviceTheme()
        {
            if (DeviceSupportsAutomaticDarkMode())
            {
                var currentUIViewController = await GetVisibleViewController();
                var userInterfaceStyle = currentUIViewController.TraitCollection.UserInterfaceStyle;
                switch (userInterfaceStyle)
                {
                    case UIUserInterfaceStyle.Dark:
                        return Theme.Dark;
                    case UIUserInterfaceStyle.Light:
                        return Theme.Light;
                }
            }

            return GetDeviceDefaultTheme();
        }

        private static Task<UIViewController> GetVisibleViewController() =>
            // UIApplication.SharedApplication can only be referenced on by Main Thread,
            // so it should be used Device.InvokeOnMainThreadAsync which was introduced in Xamarin.Forms v4.2.0
            Device.InvokeOnMainThreadAsync(() =>
            {
                var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                return rootController.PresentedViewController switch
                {
                    UINavigationController navigationController => navigationController.TopViewController,
                    UITabBarController tabBarController => tabBarController.SelectedViewController,
                    null => rootController,
                    _ => rootController.PresentedViewController,
                };
            });
    }
}
