using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string resource)
        {
            if (Uri.TryCreate(resource, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp
                    || uri.Scheme == Uri.UriSchemeHttps))
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            else
                await Application.Current.MainPage.Navigation.PushAsync(await ViewModelLocator.Instance.ResolvePageAsync(resource));
        }

        public Task NavigateBackAsync() =>
            Application.Current.MainPage.Navigation.PopAsync();

        public Task NavigateBackToRootAsync() =>
            Application.Current.MainPage.Navigation.PopToRootAsync();
    }
}
