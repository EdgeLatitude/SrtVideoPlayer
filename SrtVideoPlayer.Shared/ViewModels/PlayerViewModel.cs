using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Playback;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SrtVideoPlayer.Shared.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        private readonly IAlertsService _alertsService;
        private readonly ICommandFactoryService _commandFactoryService;
        private readonly INavigationService _navigationService;
        private readonly IPlatformInformationService _platformInformationService;

        private Dictionary<TimeSpan, string> _subtitles;

        public PlayerViewModel(
            IAlertsService alertsService,
            ICommandFactoryService commandFactoryService,
            INavigationService navigationService,
            IPlatformInformationService platformInformationService)
        {
            _alertsService = alertsService;
            _commandFactoryService = commandFactoryService;
            _navigationService = navigationService;
            _platformInformationService = platformInformationService;

            ShowHistoryCommand = _commandFactoryService.Create(ShowHistory);
            NavigateToSettingsCommand = _commandFactoryService.Create(async () => await NavigateToSettingsAsync());
            ShowAboutCommand = _commandFactoryService.Create(async () => await ShowAbout());
        }

        private string _source;

        public string Source
        {
            get => _source;
            set
            {
                if (_source == value)
                    return;
                _source = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _time;

        public TimeSpan Time
        {
            get => _time;
            set
            {
                if (_time == value)
                    return;
                _time = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadVideoCommand { get; private set; }

        public ICommand ManageInputFromHardwareCommand { get; private set; }

        public ICommand ShowHistoryCommand { get; private set; }

        public ICommand NavigateToSettingsCommand { get; private set; }

        public ICommand ShowAboutCommand { get; private set; }

        private async void ShowHistory()
        {
            if (Settings.Instance.GetPlaybackHistoryLength() == 0)
            {
                var openSettings = await _alertsService.DisplayConfirmationAsync(LocalizedStrings.Notice,
                    LocalizedStrings.DisabledResultsHistory,
                    LocalizedStrings.Settings);
                if (openSettings)
                    await NavigateToSettingsAsync();
                return;
            }

            if (!Settings.Instance.ContainsPlaybackHistory())
            {
                await _alertsService.DisplayAlertAsync(LocalizedStrings.Notice,
                    LocalizedStrings.EmptyResultsHistory);
                return;
            }

            var playbackHistory = await Settings.Instance.GetPlaybackHistoryAsync();
            playbackHistory.Reverse();

            var videosFromHistoryByName = new Dictionary<string, History>();
            foreach (var video in playbackHistory)
                videosFromHistoryByName.Add(video.Name, video);

            var videoFromHistory = await _alertsService.DisplayOptionsAsync(LocalizedStrings.History,
                LocalizedStrings.ClearHistory,
                videosFromHistoryByName.Keys.ToArray());
            if (videoFromHistory != null
                && videoFromHistory != LocalizedStrings.Cancel
                && videoFromHistory != LocalizedStrings.ClearHistory
                && videosFromHistoryByName.ContainsKey(videoFromHistory))
            {
                var videoDataFromHistory = videosFromHistoryByName[videoFromHistory];
                Source = videoDataFromHistory.Location;
                Time = videoDataFromHistory.Time;
                _subtitles = videoDataFromHistory.Subtitles;
            }
            else if (videoFromHistory == LocalizedStrings.ClearHistory)
                Settings.Instance.ClearPlaybackHistory();
        }

        private async Task NavigateToSettingsAsync() =>
            await _navigationService.NavigateToAsync(Locations.SettingsPage);

        private async Task ShowAbout() =>
            await _alertsService.DisplayAlertAsync(
                LocalizedStrings.About,
                (_platformInformationService.PlatformSupportsGettingApplicationVersion() ?
                    LocalizedStrings.AppVersion
                        + Environment.NewLine
                        + _platformInformationService.GetApplicationVersion()
                        + Environment.NewLine
                        + Environment.NewLine :
                    string.Empty)
                + LocalizedStrings.AppIconAttribution);
    }
}
