using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class CommandFactoryService : ICommandFactoryService
    {
        public ICommand Create(Action execute) =>
            new Command(execute);

        public ICommand Create<T>(Action<T> execute) =>
            new Command<T>(execute);

        public ICommand Create(Action execute, Func<bool> canExecute) =>
            new Command(execute, canExecute);

        public ICommand Create<T>(Action<T> execute, Func<T, bool> canExecute) =>
            new Command<T>(execute, canExecute);
    }
}
