using SrtVideoPlayer.Shared.PlatformServices;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class PlatformInformationService : IPlatformInformationService
    {
        public bool PlatformSupportsGettingApplicationVersion() =>
            true;

        public string GetApplicationVersion() =>
            VersionTracking.CurrentVersion;
    }
}
