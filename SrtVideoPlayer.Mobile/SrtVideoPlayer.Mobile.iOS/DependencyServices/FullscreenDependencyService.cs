using Foundation;
using SrtVideoPlayer.Mobile.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SrtVideoPlayer.Mobile.iOS.DependencyServices.FullscreenDependencyService))]
namespace SrtVideoPlayer.Mobile.iOS.DependencyServices
{
    public class FullscreenDependencyService : IFullscreenDependencyService
    {
        private const string _orientationKey = "orientation";

        public void Disable()
        {
            UIApplication.SharedApplication.StatusBarHidden = false;
            var dictionaryOfValues = UIDevice.CurrentDevice.GetDictionaryOfValuesFromKeys(new[] { new NSString(_orientationKey) });
            if (dictionaryOfValues.ContainsKey(new NSString(_orientationKey)))
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Unknown), new NSString(_orientationKey));
        }

        public void Enable(bool landscapeMode)
        {
            UIApplication.SharedApplication.StatusBarHidden = true;
            if (landscapeMode)
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeRight), new NSString(_orientationKey));
            else
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString(_orientationKey));
        }
    }
}
