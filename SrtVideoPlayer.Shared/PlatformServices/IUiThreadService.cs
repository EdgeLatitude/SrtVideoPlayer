using System;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IUiThreadService
    {
        void ExecuteOnUiThread(Action action);
    }
}
