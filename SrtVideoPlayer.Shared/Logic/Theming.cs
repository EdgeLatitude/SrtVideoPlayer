using SrtVideoPlayer.Shared.Models.Theming;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.Logic
{
    public class Theming
    {
        private static IThemingService _themingService;

        public static event EventHandler<ThemeChangeNeededEventArgs> ThemeChangeNeeded;

        public static Theming Instance
        {
            get;
            private set;
        }

        public static void Initialize(
            IThemingService themingService) =>
            Instance = new Theming(themingService);

        private Theming(
            IThemingService themingService)
        {
            _themingService = themingService;

            DeviceSupportsManualDarkMode = _themingService.DeviceSupportsManualDarkMode();
            DeviceSupportsAutomaticDarkMode = _themingService.DeviceSupportsAutomaticDarkMode();
        }

        public readonly bool DeviceSupportsManualDarkMode;

        public readonly bool DeviceSupportsAutomaticDarkMode;

        private Theme? _currentTheme;

        public async void ManageAppTheme(bool starting = false)
        {
            var theme = await GetAppOrDeviceTheme();
            if (!starting
                && theme == _currentTheme)
                return;
            ThemeChangeNeeded?.Invoke(this, new ThemeChangeNeededEventArgs { Theme = theme });
            _themingService.SetTheme(theme);
            _currentTheme = theme;
        }

        public Theme? GetAppOrDefaultTheme() =>
            Settings.Instance.ContainsTheme() ?
                Settings.Instance.GetTheme() :
                DeviceSupportsAutomaticDarkMode ?
                    (Theme?)null :
                    _themingService.GetDeviceDefaultTheme();

        public async Task<Theme> GetAppOrDeviceTheme() =>
            Settings.Instance.ContainsTheme() ?
                Settings.Instance.GetTheme() :
                await _themingService.GetDeviceTheme();
    }
}
