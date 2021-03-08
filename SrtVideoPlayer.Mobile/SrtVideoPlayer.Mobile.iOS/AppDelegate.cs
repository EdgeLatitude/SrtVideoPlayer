using Foundation;
using SrtVideoPlayer.Mobile.Pages;
using SrtVideoPlayer.Shared.Models.Files;
using SrtVideoPlayer.Shared.ViewModels;
using System.Linq;
using UIKit;

namespace SrtVideoPlayer.Mobile.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            MediaManager.CrossMediaManager.Current.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            global::Xamarin.Forms.Forms.Init();

            if (options != null
                && options.TryGetValue(UIApplication.LaunchOptionsUrlKey, out NSObject nsObject)
                && nsObject is NSUrl nsUrl)
            {
                var path = nsUrl.ToString();
                var fileManager = new NSFileManager();
                var name = fileManager.DisplayName(path);
                LoadApplication(new App(new VideoFile
                {
                    Name = name,
                    Path = path
                }));
            }
            else
                LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            if (Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Last() is PlayerPage playerPage
                && playerPage.BindingContext is PlayerViewModel playerViewModel
                && playerViewModel.Fullscreen)
                return playerViewModel.LandscapeVideo ?
                    UIInterfaceOrientationMask.LandscapeRight :
                    UIInterfaceOrientationMask.Portrait;
            return UIInterfaceOrientationMask.All;
        }
    }
}
