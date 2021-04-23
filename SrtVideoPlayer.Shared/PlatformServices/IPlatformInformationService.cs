using SrtVideoPlayer.Shared.Models.Enums;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IPlatformInformationService
    {
        bool PlatformSupportsGettingApplicationVersion();
        string GetApplicationVersion();
        DeviceOs GetDeviceOs();
    }
}
