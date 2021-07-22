using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Models.Enums;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class AlertsService : IAlertsService
    {
        public async Task DisplayAlertAsync(string title, string message) =>
            await Application.Current.MainPage.DisplayAlert(title, message, LocalizedStrings.Ok);

        public async Task<bool> DisplayConfirmationAsync(string title, string message, string accept, string cancel) =>
            await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);

        public async Task<string> DisplayOptionsAsync(string title, string destruction, params string[] options) =>
            await Application.Current.MainPage.DisplayActionSheet(title, LocalizedStrings.Cancel, destruction, options);

        public async Task<string> DisplayPromptAsync(string title, string message, string placeholder = null,
            int? maxLength = null, string initialValue = null, KeyboardType keyboardType = KeyboardType.Plain) =>
            await Application.Current.MainPage.DisplayPromptAsync(
                title,
                message,
                LocalizedStrings.Ok,
                LocalizedStrings.Cancel,
                placeholder,
                maxLength ?? -1,
                Equivalences.Keyboards.ContainsKey(keyboardType) ? Equivalences.Keyboards[keyboardType] : Keyboard.Default,
                initialValue ?? string.Empty);
    }
}
