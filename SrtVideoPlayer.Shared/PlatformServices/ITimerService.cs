using System;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface ITimerService
    {
        void StartTimer(TimeSpan interval, Func<bool> callback);
    }
}
