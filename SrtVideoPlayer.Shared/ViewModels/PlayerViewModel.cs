using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Enums;
using SrtVideoPlayer.Shared.Models.Files;
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
        private readonly IFullscreenService _fullscreenService;
        private readonly IMessagingService _messagingService;
        private readonly INavigationService _navigationService;
        private readonly IPermissionsService _permissionsService;
        private readonly IPlatformInformationService _platformInformationService;
        private readonly ITimerService _timerService;

        private readonly string[] _videoSourceOptions = new string[]
        {
            LocalizedStrings.Web,
            LocalizedStrings.Gallery,
            LocalizedStrings.Files,
        };

        private readonly string[] _subtitlesSourceOptions = new string[]
        {
            LocalizedStrings.Web,
            LocalizedStrings.Files
        };

        private TimeSpan _lastHistoryPosition = TimeSpan.Zero;
        private double? _originalVolume;

        public event EventHandler PlayPauseRequested;
        public event EventHandler SeekRequested;
        public event EventHandler StopRequested;

        public PlayerViewModel(
            IAlertsService alertsService,
            IClipboardService clipboardService,
            ICommandFactoryService commandFactoryService,
            IFileDownloaderService fileDownloaderService,
            IFilePickerService filePickerService,
            IFullscreenService fullscreenService,
            IMessagingService messagingService,
            INavigationService navigationService,
            IPermissionsService permissionsService,
            IPlatformInformationService platformInformationService,
            ITimerService timerService)
        {
            _alertsService = alertsService;
            _clipboardService = clipboardService;
            _commandFactoryService = commandFactoryService;
            _fileDownloaderService = fileDownloaderService;
            _filePickerService = filePickerService;
            _fullscreenService = fullscreenService;
            _messagingService = messagingService;
            _navigationService = navigationService;
            _permissionsService = permissionsService;
            _platformInformationService = platformInformationService;
            _timerService = timerService;

            LoadVideoCommand = _commandFactoryService.Create(async () => await LoadVideo());
            CopySubtitleToClipboardCommand = _commandFactoryService.Create(async () => await CopySubtitleToClipboard());
            ManageInputFromHardwareCommand = _commandFactoryService.Create((string character) => ManageInputFromHardware(character));
            PlayOrPauseCommand = _commandFactoryService.Create(PlayOrPause);
            StopCommand = _commandFactoryService.Create(Stop);
            GoBack5_SecondsCommand = _commandFactoryService.Create(() => Seek(false, 5));
            GoForward5_SecondsCommand = _commandFactoryService.Create(() => Seek(true, 5));
            GoBack10_SecondsCommand = _commandFactoryService.Create(() => Seek(false, 10));
            GoForward10_SecondsCommand = _commandFactoryService.Create(() => Seek(true, 10));
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

        private Video _source;

        public Video Source
        {
            get => _source;
            private set
            {
                if (_source == value)
                    return;
                _source = value;
                OnPropertyChanged();

                if (string.IsNullOrWhiteSpace(value?.Location))
                    MediaLoaded = false;
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

                ManageLastPosition(value);
            }
        }

        private TimeSpan? _duration;

        public TimeSpan? Duration
        {
            get => _duration;
            set
            {
                if (_duration == value)
                    return;
                _duration = value;
                OnPropertyChanged();
            }
        }

        private bool _subtitlesAreVisible = true;

        public bool SubtitlesAreVisible
        {
            get => _subtitlesAreVisible;
            private set
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
            private set
            {
                if (_subtitles == value)
                    return;
                _subtitles = value;
                OnPropertyChanged();

                if (value == null)
                    Subtitle = null;
                else
                    _timerService.StartTimer(TimeSpan.FromMilliseconds(500), SetCurrentSubtitle);
            }
        }

        private Subtitle _subtitle;

        public Subtitle Subtitle
        {
            get => _subtitle;
            private set
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

        private double _volume;

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

        private int _offset = Settings.Instance.GetOffset();

        public int Offset
        {
            get => _offset;
            private set
            {
                if (_offset == value)
                    return;
                _offset = value;
                OnPropertyChanged();
            }
        }

        private string _subtitleColor = Settings.Instance.GetSubtitleColor();

        public string SubtitleColor
        {
            get => _subtitleColor;
            private set
            {
                if (_subtitleColor == value)
                    return;
                _subtitleColor = value;
                OnPropertyChanged();
            }
        }

        private int _fontSize = Settings.Instance.GetFontSize();

        public int FontSize
        {
            get => _fontSize;
            private set
            {
                if (_fontSize == value)
                    return;
                _fontSize = value;
                OnPropertyChanged();
            }
        }

        private bool _buffering;

        public bool Buffering
        {
            get => _buffering;
            set
            {
                if (_buffering == value)
                    return;
                _buffering = value;
                OnPropertyChanged();
            }
        }

        private bool _fullscreen;

        public bool Fullscreen
        {
            get => _fullscreen;
            private set
            {
                if (_fullscreen == value)
                    return;
                _fullscreen = value;
                OnPropertyChanged();
            }
        }

        private bool _landscapeVideo;

        public bool LandscapeVideo
        {
            get => _landscapeVideo;
            set
            {
                if (_landscapeVideo == value)
                    return;
                _landscapeVideo = value;
                OnPropertyChanged();
            }
        }

        private bool _selectingVideo;

        public bool SelectingVideo
        {
            get => _selectingVideo;
            set
            {
                if (_selectingVideo == value)
                    return;
                _selectingVideo = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan? LastPositionFromHistory { get; set; }

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

        public async Task Launch(VideoFile videoFile = null)
        {
            var lastPosition = Settings.Instance.GetLastPosition();
            var lastPlayback = (await Settings.Instance.GetPlaybackHistoryAsync()).LastOrDefault();

            if (videoFile != null)
            {
                if (lastPosition > TimeSpan.Zero
                    && lastPlayback != null
                    && lastPlayback.Video.Location == videoFile.Path)
                    if (await _alertsService.DisplayConfirmationAsync(
                            null,
                            LocalizedStrings.RestoreLastPlaybackPrompt,
                            LocalizedStrings.Yes,
                            LocalizedStrings.No))
                    {
                        Source = lastPlayback.Video;
                        Subtitles = lastPlayback.Subtitles;
                        LastPositionFromHistory = lastPosition;
                        return;
                    }
                    else
                        Settings.Instance.SetLastPosition(TimeSpan.Zero);
                await LoadVideo(videoFile);
                return;
            }

            if (Source == null
                && lastPosition > TimeSpan.Zero
                && lastPlayback != null)
                if (await _alertsService.DisplayConfirmationAsync(
                        null,
                        LocalizedStrings.RestoreLastPlaybackPrompt,
                        LocalizedStrings.Yes,
                        LocalizedStrings.No))
                {
                    Source = lastPlayback.Video;
                    Subtitles = lastPlayback.Subtitles;
                    LastPositionFromHistory = lastPosition;
                }
                else
                    Settings.Instance.SetLastPosition(TimeSpan.Zero);
        }

        private async Task LoadVideo(VideoFile videoFile = null)
        {
            if (!await CheckAndRequestForMediaAndFilesAccessPermissions())
                return;

            SelectingVideo = true;

            Video video = default;
            if (videoFile == null)
            {
                string videoSource;
                do
                {
                    videoSource = await _alertsService.DisplayOptionsAsync(LocalizedStrings.VideoSource,
                        null,
                        _videoSourceOptions);

                    if (videoSource != null
                        && _videoSourceOptions.Contains(videoSource))
                        if (videoSource == LocalizedStrings.Web)
                            video = await LoadWebVideo();
                        else if (videoSource == LocalizedStrings.Gallery)
                            video = await LoadLocalVideoFromGallery();
                        else if (videoSource == LocalizedStrings.Files)
                            video = await LoadLocalVideoFromFiles();

                } while (videoSource != LocalizedStrings.Cancel
                    && video == null);

                if (video == null)
                {
                    SelectingVideo = false;
                    return;
                }
            }
            else
                video = new Video(videoFile.Name, videoFile.Path);

            Subtitle[] subtitles = default;
            string subtitlesSource;
            do
            {
                subtitlesSource = await _alertsService.DisplayOptionsAsync(LocalizedStrings.SubtitlesSource,
                    LocalizedStrings.NoSubtitles,
                    _subtitlesSourceOptions);

                if (subtitlesSource != null
                    && _subtitlesSourceOptions.Contains(subtitlesSource))
                {
                    if (subtitlesSource == LocalizedStrings.Web)
                        subtitles = await LoadWebSubtitles();
                    else if (subtitlesSource == LocalizedStrings.Files)
                        subtitles = await LoadLocalSubtitles();
                }
                else if (subtitlesSource == LocalizedStrings.NoSubtitles)
                    subtitles = null;

            } while (subtitlesSource != LocalizedStrings.NoSubtitles
                && subtitlesSource != LocalizedStrings.Cancel
                && subtitles == null);

            if (subtitlesSource != LocalizedStrings.NoSubtitles
                && subtitles == null)
            {
                SelectingVideo = false;
                return;
            }

            Source = video;
            Position = TimeSpan.Zero;
            Duration = null;
            Subtitles = subtitles;
            _ = Settings.Instance.ManageNewPlaybackAsync(new Playback
            {
                Video = video,
                Subtitles = subtitles
            });
        }

        private async Task<bool> CheckAndRequestForMediaAndFilesAccessPermissions()
        {
            var accessGranted = await _permissionsService.CheckMediaAndFilesAccessPermissions()
                || await _permissionsService.RequestMediaAndFilesAccessPermissions();
            if (!accessGranted)
                await _alertsService.DisplayAlertAsync(LocalizedStrings.Notice, LocalizedStrings.PleaseGrantAccessToYourGalleryAndFiles);
            return accessGranted;
        }

        private async Task<Video> LoadWebVideo()
        {
#if DEBUG
            var webSource = await PromptForWebSource("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4");
#else
            var webSource = await PromptForWebSource();
#endif
            if (string.IsNullOrWhiteSpace(webSource))
                return null;
            return new Video(General.RemoveProtocolAndSlashesFromAddress(webSource), webSource);
        }

        private async Task<Video> LoadLocalVideoFromGallery()
        {
            var file = await _filePickerService.SelectVideoFromGalleryAsync();
            if (file == null)
                return null;
            return new Video(file.Name, file.Path);
        }

        private async Task<Video> LoadLocalVideoFromFiles()
        {
            var file = await _filePickerService.SelectVideoFromFilesAsync();
            if (file == null)
                return null;
            return new Video(file.Name, file.Path);
        }

        private async Task<Subtitle[]> LoadWebSubtitles()
        {
#if DEBUG
            var webSource = await PromptForWebSource("https://raw.githubusercontent.com/moust/MediaPlayer/master/demo/subtitles.srt");
#else
            var webSource = await PromptForWebSource();
#endif
            if (string.IsNullOrWhiteSpace(webSource))
                return null;
            var filepath = await _fileDownloaderService.DownloadFileToCacheAsync(
                webSource,
                General.RemoveProtocolAndSlashesFromAddress(webSource),
                Strings.SrtFileExtension,
                true,
                LocalizedStrings.DownloadingSubtitles);
            return await General.GetSubtitlesFromContent(await File.ReadAllTextAsync(filepath));
        }

        private async Task<Subtitle[]> LoadLocalSubtitles()
        {
            var file = await _filePickerService.SelectSubtitlesAsync();
            if (file == null)
                return null;
            var subtitlesContent = await General.ReadSubtitlesFileContent(file.Path);
            if (string.IsNullOrWhiteSpace(subtitlesContent))
                return null;
            return await General.GetSubtitlesFromContent(subtitlesContent);
        }

        private async Task<string> PromptForWebSource(string initialValue = null)
        {
            var input = await _alertsService.DisplayPromptAsync(
                LocalizedStrings.WebSource,
                LocalizedStrings.EnterTheUrl,
                null,
                null,
                initialValue,
                KeyboardType.Url);

            if (string.IsNullOrWhiteSpace(input)
                || input == LocalizedStrings.Cancel)
                return null;

            input = input.Trim();

            if (!Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                await _alertsService.DisplayAlertAsync(LocalizedStrings.Notice, LocalizedStrings.PleaseEnterAValidUrl);
                return await PromptForWebSource(input);
            }

            return input;
        }

        private async Task CopySubtitleToClipboard()
        {
            var subtitle = Subtitle?.Text;
            if (!string.IsNullOrWhiteSpace(subtitle))
                await _clipboardService.SetTextAsync(subtitle);
        }

        private void ManageInputFromHardware(string character)
        {
            switch (character)
            {
                case KeyboardShortcuts.PlayPauseA:
                case KeyboardShortcuts.PlayPauseB:
                    PlayOrPause();
                    break;
                case KeyboardShortcuts.GoBack10_Seconds:
                    Seek(false, 10);
                    break;
                case KeyboardShortcuts.GoForward10_Seconds:
                    Seek(true, 10);
                    break;
                case KeyboardShortcuts.Restart:
                    Restart();
                    break;
                case KeyboardShortcuts.FullscreenOnOff:
                    FullscreenOnOff();
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
            PlayPauseRequested?.Invoke(this, new EventArgs());

        private void Stop()
        {
            StopRequested?.Invoke(this, new EventArgs());
            Source = null;
            Position = TimeSpan.Zero;
            Duration = null;
            Subtitles = null;
            if (Fullscreen)
                FullscreenOff();
        }

        private void Seek(bool forward, int seconds)
        {
            if (forward)
                Position = Position.Add(TimeSpan.FromSeconds(seconds));
            else
                Position = Position.Subtract(TimeSpan.FromSeconds(seconds));
            SeekRequested?.Invoke(this, new EventArgs());
        }

        private void Restart()
        {
            Position = TimeSpan.Zero;
            SeekRequested?.Invoke(this, new EventArgs());
        }

        private void FullscreenOnOff()
        {
            if (Fullscreen)
                FullscreenOff();
            else
                FullscreenOn();
        }

        private void FullscreenOn()
        {
            Fullscreen = true;
            _fullscreenService.Enable(this, LandscapeVideo);
        }

        private void FullscreenOff()
        {
            Fullscreen = false;
            _fullscreenService.Disable(this);
        }

        private void MuteUnmute()
        {
            if (Volume > _zero)
            {
                _originalVolume = Volume;
                Volume = _zero;
            }
            else
                Volume = _originalVolume ?? _one;
        }

        private void CaptionsOnOff() =>
            SubtitlesAreVisible = !SubtitlesAreVisible;

        private bool SetCurrentSubtitle()
        {
            var subtitles = Subtitles;
            if (subtitles == null)
            {
                Subtitle = null;
                return false;
            }

            var offset = Offset;
            var positionWithOffset = Position.Add(TimeSpan.FromMilliseconds(-offset));
            Subtitle = subtitles.LastOrDefault(subtitle =>
                subtitle.SubtitleSpan.Start <= positionWithOffset
                    && subtitle.SubtitleSpan.End >= positionWithOffset);

            var keepSetting = Subtitles != null;
            if (!keepSetting)
                Subtitle = null;
            return keepSetting;
        }

        private async Task ShowHistory()
        {
            if (Settings.Instance.GetPlaybackHistoryLength() == 0)
            {
                var openSettings = await _alertsService.DisplayConfirmationAsync(LocalizedStrings.Notice,
                    LocalizedStrings.DisabledPlaybackHistory,
                    LocalizedStrings.Settings,
                    LocalizedStrings.Cancel);
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
                Source = videoDataFromHistory.Video;
                Duration = null;
                Subtitles = videoDataFromHistory.Subtitles;
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
            Offset = Settings.Instance.GetOffset();
            SubtitleColor = Settings.Instance.GetSubtitleColor();
            FontSize = Settings.Instance.GetFontSize();
        }

        private void ManageLastPosition(TimeSpan position)
        {
            const int secondsDifferenceForSavingState = 5;
            if (Math.Abs(_lastHistoryPosition.TotalSeconds - position.TotalSeconds) < secondsDifferenceForSavingState)
                return;
            Settings.Instance.SetLastPosition(position);
            _lastHistoryPosition = position;
        }
    }
}
