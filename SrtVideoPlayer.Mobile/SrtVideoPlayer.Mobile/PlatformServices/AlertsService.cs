using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class AlertsService : IAlertsService
    {
        public async Task DisplayAlertAsync(string title, string message) =>
            await Application.Current.MainPage.DisplayAlert(title, message, LocalizedStrings.Ok);

        public async Task<bool> DisplayConfirmationAsync(string title, string message, string action) =>
            await Application.Current.MainPage.DisplayAlert(title, message, action, LocalizedStrings.Cancel);

        public async Task<string> DisplayOptionsAsync(string title, string destruction, params string[] options) =>
            await Application.Current.MainPage.DisplayActionSheet(title, LocalizedStrings.Cancel, destruction, options);
    }
}
