using Foundation;
using UIKit;

namespace SrtVideoPlayer.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            global::Xamarin.Forms.Forms.Init();

            if (options != null
                && options.TryGetValue(UIApplication.LaunchOptionsUrlKey, out NSObject nsObject)
                && nsObject is NSUrl nsUrl)
                LoadApplication(new App(nsUrl.ToString()));
            else
                LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
