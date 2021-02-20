using Foundation;
using SrtVideoPlayer.Shared.Models.Files;
using System.IO;
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
                var file = File.OpenRead(path);
                var name = file.Name;
                file.Close();
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
    }
}
