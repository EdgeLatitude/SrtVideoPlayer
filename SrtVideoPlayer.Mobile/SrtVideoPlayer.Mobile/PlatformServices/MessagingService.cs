using SrtVideoPlayer.Shared.PlatformServices;
using SrtVideoPlayer.Shared.ViewModels;
using System;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class MessagingService : IMessagingService
    {
        public void Subscribe(BaseViewModel subscriber, string name, Action<BaseViewModel> callback, BaseViewModel sender = null) =>
            MessagingCenter.Subscribe(subscriber, name, callback, sender);

        public void Unsubscribe(BaseViewModel subscriber, string name) =>
            MessagingCenter.Unsubscribe<BaseViewModel>(subscriber, name);

        public void Send(BaseViewModel sender, string name) =>
            MessagingCenter.Send(sender, name);
    }
}
