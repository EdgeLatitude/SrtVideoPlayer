using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SrtVideoPlayer.Shared.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        #region Fields
        private readonly INavigationService _navigationService;
        #endregion

        #region Properties
        private bool _platformSupportsGettingApplicationVersion;

        public bool PlatformSupportsGettingApplicationVersion
        {
            get => _platformSupportsGettingApplicationVersion;
            private set
            {
                if (_platformSupportsGettingApplicationVersion == value)
                    return;
                _platformSupportsGettingApplicationVersion = value;
                OnPropertyChanged();
            }
        }

        private string _appVersion;

        public string AppVersion
        {
            get => _appVersion;
            private set
            {
                if (_appVersion == value)
                    return;
                _appVersion = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand NavigateToWebsiteCommand { get; }
        #endregion

        #region Constructors
        public AboutViewModel(
            ICommandFactoryService commandFactoryService,
            INavigationService navigationService,
            IPlatformInformationService platformInformationService)
        {
            _navigationService = navigationService;

            NavigateToWebsiteCommand = commandFactoryService.Create(async (string website) => await NavigateToWebsiteAsync(website));

            PlatformSupportsGettingApplicationVersion = platformInformationService.PlatformSupportsGettingApplicationVersion();
            if (PlatformSupportsGettingApplicationVersion)
                AppVersion = platformInformationService.GetApplicationVersion();
        }
        #endregion

        #region Methods
        private async Task NavigateToWebsiteAsync(string website) =>
            await _navigationService.NavigateToAsync($"{Strings.HttpsProtocol}{website}");
        #endregion
    }
}
