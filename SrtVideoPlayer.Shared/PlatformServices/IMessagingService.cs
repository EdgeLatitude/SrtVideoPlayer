using SrtVideoPlayer.Shared.ViewModels;
using System;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IMessagingService
    {
        void Subscribe(BaseViewModel subscriber, string name, Action<BaseViewModel> callback, BaseViewModel sender = null);
        void Unsubscribe(BaseViewModel subscriber, string name);
        void Send(BaseViewModel sender, string name);
    }
}
