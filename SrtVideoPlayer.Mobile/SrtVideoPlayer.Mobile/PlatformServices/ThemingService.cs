using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Mobile.Themes;
using SrtVideoPlayer.Shared.Models.Theming;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class ThemingService : IThemingService
    {
        private static readonly IThemingDependencyService _themingDependencyService
            = DependencyService.Get<IThemingDependencyService>();

        public bool DeviceSupportsManualDarkMode() =>
            _themingDependencyService.DeviceSupportsManualDarkMode();

        public bool DeviceSupportsAutomaticDarkMode() =>
            _themingDependencyService.DeviceSupportsAutomaticDarkMode();

        public Theme GetDeviceDefaultTheme() =>
            _themingDependencyService.GetDeviceDefaultTheme();

        public async Task<Theme> GetDeviceTheme() =>
            await _themingDependencyService.GetDeviceTheme();

        public void SetTheme(Theme theme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            var appJustLaunched = true;
            if (mergedDictionaries != null)
            {
                if (mergedDictionaries.Any())
                    appJustLaunched = false;
                if (!appJustLaunched)
                    mergedDictionaries.Clear();
                switch (theme)
                {
                    case Theme.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    case Theme.Light:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }
            }

            if (!appJustLaunched
                && _themingDependencyService.DeviceRequiresPagesRedraw())
                RedrawPages();
        }

        private void RedrawPages()
        {
            var firstPage = Application.Current.MainPage.Navigation.NavigationStack[0];
            var originalCount = Application.Current.MainPage.Navigation.NavigationStack.Count;
            for (int i = 0; i < originalCount; i++)
            {
                var page = Application.Current.MainPage.Navigation.NavigationStack[i * 2];
                Application.Current.MainPage.Navigation.InsertPageBefore((Page)Activator.CreateInstance(page.GetType()), firstPage);
            }
            var newCount = Application.Current.MainPage.Navigation.NavigationStack.Count;
            if (newCount == originalCount * 2)
            {
                var pageToPopIndex = originalCount;
                while (Application.Current.MainPage.Navigation.NavigationStack.Count - 1 > pageToPopIndex)
                    Application.Current.MainPage.Navigation.RemovePage(
                        Application.Current.MainPage.Navigation.NavigationStack[pageToPopIndex]);
                Application.Current.MainPage.Navigation.PopAsync(false);
            }
        }
    }
}
