using SrtVideoPlayer.Shared.Models.Enums;
using SrtVideoPlayer.Shared.PlatformServices;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class PlatformInformationService : IPlatformInformationService
    {
        public bool PlatformSupportsGettingApplicationVersion() =>
            true;

        public string GetApplicationVersion() =>
            VersionTracking.CurrentVersion;

        public DeviceOs GetDeviceOs() =>
            Equivalences.DeviceOss.ContainsKey(Device.RuntimePlatform) ?
            Equivalences.DeviceOss[Device.RuntimePlatform] :
            DeviceOs.Undefined;
    }
}
