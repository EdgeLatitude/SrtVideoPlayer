using Autofac;
using SrtVideoPlayer.Mobile.PlatformServices;
using SrtVideoPlayer.Shared.PlatformServices;
using SrtVideoPlayer.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace SrtVideoPlayer.Mobile
{
    internal class ViewModelLocator
    {
        public static ViewModelLocator Instance
        {
            get;
            private set;
        }

        public static void Initialize() =>
            Instance = new ViewModelLocator();

        public TViewModel ResolveViewModel<TViewModel>()
            where TViewModel : BaseViewModel =>
            _container.Resolve<TViewModel>();

        public T Resolve<T>() =>
            _container.Resolve<T>();

        private readonly IContainer _container;

        private readonly Dictionary<Type, Type> _implementationInterfaceDictionary = new Dictionary<Type, Type>
        {
            { typeof(AlertsService), typeof(IAlertsService) },
            { typeof(ClipboardService), typeof(IClipboardService) },
            { typeof(CommandFactoryService), typeof(ICommandFactoryService) },
            { typeof(FileDownloaderService), typeof(IFileDownloaderService) },
            { typeof(FilePickerService), typeof(IFilePickerService) },
            { typeof(FullscreenService), typeof(IFullscreenService) },
            { typeof(MessagingService), typeof(IMessagingService) },
            { typeof(NavigationService), typeof(INavigationService) },
            { typeof(PermissionsService), typeof(IPermissionsService) },
            { typeof(PlatformInformationService), typeof(IPlatformInformationService) },
            { typeof(SettingsService), typeof(ISettingsService) },
            { typeof(ThemingService), typeof(IThemingService) },
            { typeof(TimerService), typeof(ITimerService) },
            { typeof(UiThreadService), typeof(IUiThreadService) }
        };

        private readonly Type[] _viewModelsToResolve = new Type[]
        {
            typeof(AboutViewModel)
        };

        private readonly Type[] _viewModelsToResolveAsSingletons = new Type[]
        {
            typeof(PlayerViewModel),
            typeof(SettingsViewModel)
        };

        private ViewModelLocator()
        {
            var builder = new ContainerBuilder();
            RegisterPlatformServices(builder);
            RegisterViewModels(builder);
            RegisterLogic(builder);
            _container = builder.Build();
        }

        private void RegisterPlatformServices(ContainerBuilder builder)
        {
            foreach (var implementationInterfacePair in _implementationInterfaceDictionary)
                builder.RegisterType(implementationInterfacePair.Key)
                    .As(implementationInterfacePair.Value).SingleInstance();
        }

        private void RegisterViewModels(ContainerBuilder builder)
        {
            foreach (var viewModelToResolve in _viewModelsToResolve)
                builder.RegisterType(viewModelToResolve);
            foreach (var viewModelToResolve in _viewModelsToResolveAsSingletons)
                builder.RegisterType(viewModelToResolve).SingleInstance();
        }

        private void RegisterLogic(ContainerBuilder builder)
        {
            builder.RegisterType<Shared.Logic.Settings>().InstancePerLifetimeScope();
            builder.RegisterType<Shared.Logic.Theming>().InstancePerLifetimeScope();
        }
    }
}
