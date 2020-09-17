using Autofac;
using SrtVideoPlayer.Mobile.PlatformServices;
using SrtVideoPlayer.Shared.PlatformServices;
using SrtVideoPlayer.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace SrtVideoPlayer.Mobile
{
    class ViewModelLocator
    {
        public static ViewModelLocator Instance
        {
            get;
            private set;
        }

        public static void Initialize() =>
            Instance = new ViewModelLocator();

        private readonly IContainer _container;

        private readonly Dictionary<Type, Type> _implementationInterfaceDictionary = new Dictionary<Type, Type>
        {
            { typeof(AlertsService), typeof(IAlertsService) },
            { typeof(ClipboardService), typeof(IClipboardService) },
            { typeof(CommandFactoryService), typeof(ICommandFactoryService) },
            { typeof(FilePickerService), typeof(IFilePickerService) },
            { typeof(NavigationService), typeof(INavigationService) },
            { typeof(PlatformInformationService), typeof(IPlatformInformationService) },
            { typeof(SettingsService), typeof(ISettingsService) },
            { typeof(ThemingService), typeof(IThemingService) },
            { typeof(UiThreadService), typeof(IUiThreadService) }
        };

        private readonly Type[] _viewModelsToResolve = new Type[]
        {
            typeof(PlayerViewModel),
            typeof(SettingsViewModel)
        };

        private ViewModelLocator()
        {
            var builder = new ContainerBuilder();
            RegisterPlatformServices(builder);
            RegisterViewModels(builder);
            _container = builder.Build();
            InitializeSingletons();
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
                builder.RegisterType(viewModelToResolve).SingleInstance();
        }

        private void InitializeSingletons()
        {
            Shared.Logic.Settings.Initialize(
                _container.Resolve<ISettingsService>(),
                _container.Resolve<IThemingService>());
            Shared.Logic.Theming.Initialize(
                _container.Resolve<IThemingService>());
        }

        public TViewModel Resolve<TViewModel>()
            where TViewModel : BaseViewModel =>
            _container.Resolve<TViewModel>();
    }
}
