using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Enums;
using SrtVideoPlayer.Shared.Models.Playback;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SrtVideoPlayer.Shared.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        private const double _one = 1d;
        private const double _zero = 0d;

        private readonly IAlertsService _alertsService;
        private readonly IClipboardService _clipboardService;
        private readonly ICommandFactoryService _commandFactoryService;
        private readonly IFileDownloaderService _fileDownloaderService;
        private readonly IFilePickerService _filePickerService;
        private readonly IMessagingService _messagingService;
        private readonly INavigationService _navigationService;
        private readonly IPermissionsService _permissionsService;
        private readonly IPlatformInformationService _platformInformationService;

        private readonly string[] _mediaSourceOptions = new string[]
        {
            LocalizedStrings.Web,
            LocalizedStrings.LocalStorage
        };

        public event EventHandler PlayPauseRequested;
        public event EventHandler StopRequested;
        public event EventHandler FullscreenOnOffRequested;
        public event EventHandler FullscreenOnRequested;
        public event EventHandler FullscreenOffRequested;

        public PlayerViewModel(
            IAlertsService alertsService,
            IClipboardService clipboardService,
            ICommandFactoryService commandFactoryService,
            IFileDownloaderService fileDownloaderService,
            IFilePickerService filePickerService,
            IMessagingService messagingService,
            INavigationService navigationService,
            IPermissionsService permissionsService,
            IPlatformInformationService platformInformationService)
        {
            _alertsService = alertsService;
            _clipboardService = clipboardService;
            _commandFactoryService = commandFactoryService;
            _fileDownloaderService = fileDownloaderService;
            _filePickerService = filePickerService;
            _messagingService = messagingService;
            _navigationService = navigationService;
            _permissionsService = permissionsService;
            _platformInformationService = platformInformationService;

            LoadVideoCommand = _commandFactoryService.Create(async () => await LoadVideo());
            CopySubtitleToClipboardCommand = _commandFactoryService.Create(CopySubtitleToClipboard);
            ManageInputFromHardwareCommand = _commandFactoryService.Create((string character) => ManageInputFromHardware(character));
            PlayOrPauseCommand = _commandFactoryService.Create(PlayOrPause);
            StopCommand = _commandFactoryService.Create(Stop);
            GoBack5_SecondsCommand = _commandFactoryService.Create(GoBack5_Seconds);
            GoForward5_SecondsCommand = _commandFactoryService.Create(GoForward5_Seconds);
            GoBack10_SecondsCommand = _commandFactoryService.Create(GoBack10_Seconds);
            GoForward10_SecondsCommand = _commandFactoryService.Create(GoForward10_Seconds);
            RestartCommand = _commandFactoryService.Create(Restart);
            FullscreenOnOffCommand = _commandFactoryService.Create(FullscreenOnOff);
            FullscreenOnCommand = _commandFactoryService.Create(FullscreenOn);
            FullscreenOffCommand = _commandFactoryService.Create(FullscreenOff);
            MuteUnmuteCommand = _commandFactoryService.Create(MuteUnmute);
            CaptionsOnOffCommand = _commandFactoryService.Create(CaptionsOnOff);
            ShowHistoryCommand = _commandFactoryService.Create(async () => await ShowHistory());
            NavigateToSettingsCommand = _commandFactoryService.Create(async () => await NavigateToSettingsAsync());
            ShowAboutCommand = _commandFactoryService.Create(async () => await ShowAbout());

            _messagingService.Subscribe(this, Strings.SettingsChanged, (viewmodel) => RefreshFromSettings());
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

                MediaLoaded = !string.IsNullOrWhiteSpace(value);
            }
        }

        private TimeSpan _position = TimeSpan.Zero;

        public TimeSpan Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    return;
                _position = value;
                OnPropertyChanged();
            }
        }

        private bool _subtitlesAreVisible = true;

        public bool SubtitlesAreVisible
        {
            get => _subtitlesAreVisible;
            set
            {
                if (_subtitlesAreVisible == value)
                    return;
                _subtitlesAreVisible = value;
                OnPropertyChanged();
            }
        }

        private Subtitle[] _subtitles;

        public Subtitle[] Subtitles
        {
            get => _subtitles;
            set
            {
                if (_subtitles == value)
                    return;
                _subtitles = value;
                OnPropertyChanged();
            }
        }

        private Subtitle _subtitle;

        public Subtitle Subtitle
        {
            get => _subtitle;
            set
            {
                if (_subtitle == value)
                    return;
                _subtitle = value;
                OnPropertyChanged();
            }
        }

        private bool _mediaLoaded;

        public bool MediaLoaded
        {
            get => _mediaLoaded;
            set
            {
                if (_mediaLoaded == value)
                    return;
                _mediaLoaded = value;
                OnPropertyChanged();
            }
        }

        private double _volume = _one;

        public double Volume
        {
            get => _volume;
            set
            {
                if (_volume == value)
                    return;
                _volume = value;
                OnPropertyChanged();
            }
        }

        public string SubtitleColor =>
            Settings.Instance.GetSubtitleColor();

        public int FontSize =>
            Settings.Instance.GetFontSize();

        public int Offset =>
            Settings.Instance.GetOffset();

        public ICommand LoadVideoCommand { get; }

        public ICommand CopySubtitleToClipboardCommand { get; }

        public ICommand ManageInputFromHardwareCommand { get; }

        public ICommand PlayOrPauseCommand { get; }

        public ICommand StopCommand { get; }

        public ICommand GoBack5_SecondsCommand { get; }

        public ICommand GoForward5_SecondsCommand { get; }

        public ICommand GoBack10_SecondsCommand { get; }

        public ICommand GoForward10_SecondsCommand { get; }

        public ICommand RestartCommand { get; }

        public ICommand FullscreenOnOffCommand { get; }

        public ICommand FullscreenOnCommand { get; }

        public ICommand FullscreenOffCommand { get; }

        public ICommand MuteUnmuteCommand { get; }

        public ICommand CaptionsOnOffCommand { get; }

        public ICommand ShowHistoryCommand { get; }

        public ICommand NavigateToSettingsCommand { get; }

        public ICommand ShowAboutCommand { get; }

        public async Task LoadVideo(string videoUri = null)
        {
            if (!await CheckAndRequestMediaAccessPermission())
                return;

            Video video;
            if (string.IsNullOrWhiteSpace(videoUri))
            {
                var videoSource = await _alertsService.DisplayOptionsAsync(LocalizedStrings.VideoSource,
                    null,
                    _mediaSourceOptions);
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
            }
            else
                video = new Video(General.RemoveProtocolAndSlashesFromAddress(videoUri), videoUri);

            var subtitlesSource = await _alertsService.DisplayOptionsAsync(LocalizedStrings.SubtitlesSource,
                LocalizedStrings.NoSubtitles,
                _mediaSourceOptions);
            Subtitle[] subtitles;
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
            Position = TimeSpan.Zero;
            Subtitles = subtitles;
            _ = Settings.Instance.ManageNewPlaybackAsync(new Playback
            {
                Video = video,
                Time = TimeSpan.Zero,
                Subtitles = subtitles
            });
        }

        private async Task<bool> CheckAndRequestMediaAccessPermission()
        {
            var accessGranted = await _permissionsService.CheckMediaAccessPermission()
                || await _permissionsService.RequestMediaAccessPermission();
            if (!accessGranted)
                await _alertsService.DisplayAlertAsync(LocalizedStrings.Notice, LocalizedStrings.PleaseGrantAccessToYourMediaFiles);
            return accessGranted;
        }

        private async Task<Video> LoadWebVideo()
        {
            var webSource = await PromptForWebSource();
            if (string.IsNullOrWhiteSpace(webSource))
                return null;
            return new Video(General.RemoveProtocolAndSlashesFromAddress(webSource), webSource);
        }

        private async Task<Video> LoadLocalVideo()
        {
            var filepath = await _filePickerService.SelectVideoAsync();
            if (string.IsNullOrWhiteSpace(filepath))
                return null;
            return new Video(General.RemoveProtocolAndSlashesFromAddress(filepath), filepath);
        }

        private async Task<Subtitle[]> LoadWebSubtitles()
        {
            const string srtExtension = "srt";
            var webSource = await PromptForWebSource();
            if (string.IsNullOrWhiteSpace(webSource))
                return null;
            var filepath = await _fileDownloaderService.DownloadFileToCacheAsync(webSource, General.RemoveProtocolAndSlashesFromAddress(webSource), srtExtension, true);
            return await General.GetSubtitlesFromContent(await File.ReadAllTextAsync(filepath));
        }

        private async Task<Subtitle[]> LoadLocalSubtitles()
        {
            var subtitlesContent = await _filePickerService.ReadSubtitlesAsync();
            if (string.IsNullOrWhiteSpace(subtitlesContent))
                return null;
            return await General.GetSubtitlesFromContent(subtitlesContent);
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

        private async void CopySubtitleToClipboard() =>
            await _clipboardService.SetTextAsync(Subtitle?.Text);

        private void ManageInputFromHardware(string character)
        {
            switch (character)
            {
                case KeyboardShortcuts.PlayPauseA:
                case KeyboardShortcuts.PlayPauseB:
                    PlayOrPause();
                    break;
                case KeyboardShortcuts.GoBack10_Seconds:
                    GoBack10_Seconds();
                    break;
                case KeyboardShortcuts.GoForward10_Seconds:
                    GoForward10_Seconds();
                    break;
                case KeyboardShortcuts.Restart:
                    Restart();
                    break;
                case KeyboardShortcuts.FullscreenOn:
                    FullscreenOn();
                    break;
                case KeyboardShortcuts.MuteUnmute:
                    MuteUnmute();
                    break;
                case KeyboardShortcuts.CaptionsOnOff:
                    CaptionsOnOff();
                    break;
            }
        }

        private void PlayOrPause() =>
            PlayPauseRequested.Invoke(this, new EventArgs());

        private void Stop()
        {
            StopRequested.Invoke(this, new EventArgs());
            Source = null;
        }

        private void GoBack5_Seconds()
        {
            const double shortcutTimeSpanSeconds = 5d;
            Position = Position.Subtract(TimeSpan.FromSeconds(shortcutTimeSpanSeconds));
        }

        private void GoForward5_Seconds()
        {
            const double shortcutTimeSpanSeconds = 5d;
            Position = Position.Add(TimeSpan.FromSeconds(shortcutTimeSpanSeconds));
        }

        private void GoBack10_Seconds()
        {
            const double shortcutTimeSpanSeconds = 10d;
            Position = Position.Subtract(TimeSpan.FromSeconds(shortcutTimeSpanSeconds));
        }

        private void GoForward10_Seconds()
        {
            const double shortcutTimeSpanSeconds = 10d;
            Position = Position.Add(TimeSpan.FromSeconds(shortcutTimeSpanSeconds));
        }

        private void Restart() =>
            Position = TimeSpan.Zero;

        private void FullscreenOnOff() =>
            FullscreenOnOffRequested.Invoke(this, new EventArgs());

        private void FullscreenOn() =>
            FullscreenOnRequested.Invoke(this, new EventArgs());

        private void FullscreenOff() =>
            FullscreenOffRequested.Invoke(this, new EventArgs());

        private void MuteUnmute() =>
            Volume = Volume == _one ?
                _zero :
                _one;

        private void CaptionsOnOff() =>
            SubtitlesAreVisible = !SubtitlesAreVisible;

        private void SetCurrentSubtitle()
        {
            var offset = Offset;
            var positionWithOffset = Position.Add(TimeSpan.FromMilliseconds(-offset));
            Subtitle = _subtitles.LastOrDefault(subtitle =>
                subtitle.SubtitleSpan.Start <= positionWithOffset
                    && subtitle.SubtitleSpan.End >= positionWithOffset);
        }

        private async Task ShowHistory()
        {
            if (Settings.Instance.GetPlaybackHistoryLength() == 0)
            {
                var openSettings = await _alertsService.DisplayConfirmationAsync(LocalizedStrings.Notice,
                    LocalizedStrings.DisabledPlaybackHistory,
                    LocalizedStrings.Settings);
                if (openSettings)
                    await NavigateToSettingsAsync();
                return;
            }

            if (!Settings.Instance.ContainsPlaybackHistory())
            {
                await _alertsService.DisplayAlertAsync(LocalizedStrings.Notice,
                    LocalizedStrings.EmptyPlaybackHistory);
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
                Position = videoDataFromHistory.Time;
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

        private void RefreshFromSettings()
        {
            OnPropertyChanged(nameof(SubtitleColor));
            OnPropertyChanged(nameof(FontSize));
        }
    }
}
