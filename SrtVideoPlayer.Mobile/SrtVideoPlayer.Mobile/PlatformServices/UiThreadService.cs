using SrtVideoPlayer.Shared.PlatformServices;
using System;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class UiThreadService : IUiThreadService
    {
        public void ExecuteOnUiThread(Action action) =>
            Device.BeginInvokeOnMainThread(action);
    }
}
