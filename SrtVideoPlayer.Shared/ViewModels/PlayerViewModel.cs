using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Enums;
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
        private readonly IFileDownloaderService _fileDownloaderService;
        private readonly INavigationService _navigationService;
        private readonly IPlatformInformationService _platformInformationService;

        private readonly string[] _mediaSourceOptions = new string[]
        {
            LocalizedStrings.Web,
            LocalizedStrings.LocalStorage
        };

        private Dictionary<TimeSpan, string> _subtitles;

        public PlayerViewModel(
            IAlertsService alertsService,
            ICommandFactoryService commandFactoryService,
            IFileDownloaderService fileDownloaderService,
            INavigationService navigationService,
            IPlatformInformationService platformInformationService)
        {
            _alertsService = alertsService;
            _commandFactoryService = commandFactoryService;
            _fileDownloaderService = fileDownloaderService;
            _navigationService = navigationService;
            _platformInformationService = platformInformationService;

            LoadVideoCommand = _commandFactoryService.Create(async () => await LoadVideo());
            ShowHistoryCommand = _commandFactoryService.Create(async () => await ShowHistory());
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

        public ICommand LoadVideoCommand { get; }

        public ICommand ManageInputFromHardwareCommand { get; }

        public ICommand PlayOrPauseCommand { get; }

        public ICommand GoBack5_SecondsCommand { get; }

        public ICommand GoForward5_SecondsCommand { get; }

        public ICommand ExitFullScreenCommand { get; }

        public ICommand ShowHistoryCommand { get; }

        public ICommand NavigateToSettingsCommand { get; }

        public ICommand ShowAboutCommand { get; }

        private async Task LoadVideo()
        {
            var videoSource = await _alertsService.DisplayOptionsAsync(LocalizedStrings.VideoSource,
                null,
                _mediaSourceOptions);
            Video video;
            if (videoSource != null
                && _mediaSourceOptions.Contains(videoSource))
                if (videoSource == LocalizedStrings.Web)
                    video = await LoadWebVideo();
                else if (videoSource == LocalizedStrings.LocalStorage)
                    video = await LoadLocalVideo();
                else
                    return;
            else
                return;

            if (video == null)
                return;

            var subtitlesSource = await _alertsService.DisplayOptionsAsync(LocalizedStrings.SubtitlesSource,
                LocalizedStrings.NoSubtitles,
                _mediaSourceOptions);
            Dictionary<TimeSpan, string> subtitles;
            if (subtitlesSource != null
                && _mediaSourceOptions.Contains(subtitlesSource))
                if (subtitlesSource == LocalizedStrings.Web)
                    subtitles = await LoadWebSubtitles();
                else if (subtitlesSource == LocalizedStrings.LocalStorage)
                    subtitles = await LoadLocalSubtitles();
                else
                    return;
            else if (subtitlesSource == LocalizedStrings.NoSubtitles)
                subtitles = null;
            else
                return;

            if (subtitlesSource != LocalizedStrings.NoSubtitles
                && subtitles == null)
                return;

            Source = video.Location;
            Time = TimeSpan.Zero;
            _subtitles = subtitles;
            _ = Settings.Instance.ManageNewPlaybackAsync(new Playback
            {
                Video = video,
                Time = TimeSpan.Zero,
                Subtitles = subtitles
            });
        }

        private async Task<Video> LoadWebVideo()
        {
            var webSource = await PromptForWebSource();
            if (string.IsNullOrWhiteSpace(webSource))
                return null;
        }

        private async Task<Video> LoadLocalVideo()
        {

        }

        private async Task<Dictionary<TimeSpan, string>> LoadWebSubtitles()
        {
            var webSource = await PromptForWebSource();
            if (string.IsNullOrWhiteSpace(webSource))
                return null;
        }

        private async Task<Dictionary<TimeSpan, string>> LoadLocalSubtitles()
        {

        }

        private async Task<string> PromptForWebSource()
        {
            var input = await _alertsService.DisplayPromptAsync(
                LocalizedStrings.WebSource,
                LocalizedStrings.EnterTheUrl,
                null,
                null,
                null,
                KeyboardType.Url);

            if (string.IsNullOrWhiteSpace(input)
                || input == LocalizedStrings.Cancel)
                return null;

            input = input.Trim();

            if (!Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                await _alertsService.DisplayAlertAsync(LocalizedStrings.Notice, LocalizedStrings.PleaseEnterAValidUrl);
                return await PromptForWebSource();
            }

            return input;
        }

        private async Task ShowHistory()
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

            var videosFromHistoryByName = new Dictionary<string, Playback>();
            foreach (var playback in playbackHistory)
                videosFromHistoryByName.Add(playback.Video.Name, playback);

            var videoFromHistory = await _alertsService.DisplayOptionsAsync(LocalizedStrings.History,
                LocalizedStrings.ClearHistory,
                videosFromHistoryByName.Keys.ToArray());
            if (videoFromHistory != null
                && videosFromHistoryByName.ContainsKey(videoFromHistory))
            {
                var videoDataFromHistory = videosFromHistoryByName[videoFromHistory];
                Source = videoDataFromHistory.Video.Location;
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
