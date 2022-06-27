using SrtVideoPlayer.Shared.Models.Theming;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IThemingService
    {
        bool DeviceSupportsManualDarkMode();
        bool DeviceSupportsAutomaticDarkMode();
        Theme GetDeviceDefaultTheme();
        Task<Theme> GetDeviceThemeAsync();
        Task SetThemeAsync(Theme theme);
    }
}
