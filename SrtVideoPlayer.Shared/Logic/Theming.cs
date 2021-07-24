﻿using SrtVideoPlayer.Shared.Models.Theming;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.Logic
{
    public class Theming
    {
        public static event EventHandler<ThemeChangeNeededEventArgs> ThemeChangeNeeded;

        private readonly Settings _settings;

        private readonly IThemingService _themingService;

        private Theme? _currentTheme;

        public bool DeviceSupportsManualDarkMode { get; }

        public bool DeviceSupportsAutomaticDarkMode { get; }

        public Theming(
            Settings settings,
            IThemingService themingService)
        {
            _settings = settings;
            _themingService = themingService;

            DeviceSupportsManualDarkMode = _themingService.DeviceSupportsManualDarkMode();
            DeviceSupportsAutomaticDarkMode = _themingService.DeviceSupportsAutomaticDarkMode();
        }

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
            _settings.ContainsTheme() ?
                _settings.GetTheme() :
                DeviceSupportsAutomaticDarkMode ?
                    (Theme?)null :
                    _themingService.GetDeviceDefaultTheme();

        public async Task<Theme> GetAppOrDeviceTheme() =>
            _settings.ContainsTheme() ?
                _settings.GetTheme() :
                await _themingService.GetDeviceTheme();
    }
}
