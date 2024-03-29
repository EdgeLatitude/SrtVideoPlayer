﻿using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Models.Enums;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class AlertsService : IAlertsService
    {
        public Task DisplayAlertAsync(string title, string message) =>
            Application.Current.MainPage.DisplayAlert(title, message, LocalizedStrings.Ok);

        public Task<bool> DisplayConfirmationAsync(string title, string message, string accept, string cancel) =>
            Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);

        public Task<string> DisplayOptionsAsync(string title, string destruction, params string[] options) =>
            Application.Current.MainPage.DisplayActionSheet(title, LocalizedStrings.Cancel, destruction, options);

        public Task<string> DisplayPromptAsync(string title, string message, string placeholder = null,
            int? maxLength = null, string initialValue = null, KeyboardType keyboardType = KeyboardType.Plain) =>
            Application.Current.MainPage.DisplayPromptAsync(
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
