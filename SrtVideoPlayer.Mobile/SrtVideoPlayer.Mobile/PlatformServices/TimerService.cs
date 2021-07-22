using SrtVideoPlayer.Shared.PlatformServices;
using System;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class TimerService : ITimerService
    {
        public void StartTimer(TimeSpan interval, Func<bool> callback) =>
            Device.StartTimer(interval, callback);
    }
}
