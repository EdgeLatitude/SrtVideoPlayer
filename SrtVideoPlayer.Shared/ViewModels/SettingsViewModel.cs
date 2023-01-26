using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Extensions;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Theming;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SrtVideoPlayer.Shared.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Fields
        private bool _loaded;
        private int _currentHistoryLength;
        private int _currentFontSize;
        private int _currentOffset;
        private string _currentSubtitleColor;
        private Theme? _currentTheme;

        private readonly Settings _settings;
        private readonly Theming _theming;

        private readonly IMessagingService _messagingService;

        private readonly IDictionary<string, Theme?> _themesDictionary = new Dictionary<string, Theme?>
        {
            { LocalizedStrings.Light, Theme.Light },
            { LocalizedStrings.Dark, Theme.Dark }
        };

        private readonly IReadOnlyDictionary<string, string> _subtitleColorsDictionary = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { LocalizedStrings.Yellow, Colors.Yellow },
            { LocalizedStrings.White, Colors.White },
            { LocalizedStrings.Cyan, Colors.Cyan }
        });

        private const string _minusCharacter = "-";
        #endregion

        #region Properties
        private string _historyLength;

        public string HistoryLength
        {
            get => _historyLength;
            set
            {
                if (_historyLength == value)
                    return;
                if (!string.IsNullOrEmpty(value)
                    && !value.All(c => char.IsNumber(c)))
                {
                    HistoryLength = _historyLength;
                    OnPropertyChanged();
                    return;
                }
                _historyLength = value;
                SettingsChanged = true;
                OnPropertyChanged();
            }
        }

        private bool _deviceSupportManualDarkMode;

        public bool DeviceSupportsManualDarkMode
        {
            get => _deviceSupportManualDarkMode;
            private set
            {
                if (_deviceSupportManualDarkMode == value)
                    return;
                _deviceSupportManualDarkMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StyleSectionIsVisible));
            }
        }

        private string[] _themes;

        public string[] Themes
        {
            get => _themes;
            private set
            {
                if (_themes == value)
                    return;
                _themes = value;
                OnPropertyChanged();
            }
        }

        private string _selectedTheme;

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme == value)
                    return;
                _selectedTheme = value;
                SettingsChanged = true;
                OnPropertyChanged();
            }
        }

        private string[] _subtitleColors;

        public string[] SubtitleColors
        {
            get => _subtitleColors;
            private set
            {
                if (_subtitleColors == value)
                    return;
                _subtitleColors = value;
                OnPropertyChanged();
            }
        }

        private string _selectedSubtitleColor;

        public string SelectedSubtitleColor
        {
            get => _selectedSubtitleColor;
            set
            {
                if (_selectedSubtitleColor == value)
                    return;
                if (string.IsNullOrWhiteSpace(value)
                    || !_subtitleColorsDictionary.ContainsKey(value))
                {
                    SelectedSubtitleColor = _selectedSubtitleColor;
                    OnPropertyChanged();
                    return;
                }
                _selectedSubtitleColor = value;
                SettingsChanged = true;
                OnPropertyChanged();
                SubtitleColorPreview = _subtitleColorsDictionary[value];
            }
        }

        private string _fontSize;

        public string FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize == value)
                    return;
                if (!string.IsNullOrEmpty(value)
                    && !value.All(c => char.IsNumber(c)))
                {
                    FontSize = _fontSize;
                    OnPropertyChanged();
                    return;
                }
                _fontSize = value;
                SettingsChanged = true;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                    FontSizePreview = int.Parse(value);
            }
        }

        private string _offset;

        public string Offset
        {
            get => _offset;
            set
            {
                if (_offset == value)
                    return;
                if (!string.IsNullOrEmpty(value)
                    && value != _minusCharacter
                    && !int.TryParse(value, out var _))
                {
                    Offset = _offset;
                    OnPropertyChanged();
                    return;
                }
                _offset = value;
                SettingsChanged = true;
                OnPropertyChanged();
            }
        }

        private bool _settingsChanged;

        public bool SettingsChanged
        {
            get => _settingsChanged;
            private set
            {
                value &= _loaded;
                if (_settingsChanged == value)
                    return;
                _settingsChanged = value;
                SaveSettingsCommand.CanExecute(null);
                OnPropertyChanged();
            }
        }

        private string _subtitleColorPreview;

        public string SubtitleColorPreview
        {
            get => _subtitleColorPreview;
            private set
            {
                if (_subtitleColorPreview == value)
                    return;
                _subtitleColorPreview = value;
                OnPropertyChanged();
            }
        }

        private int _fontSizePreview;

        public int FontSizePreview
        {
            get => _fontSizePreview;
            private set
            {
                if (_fontSizePreview == value)
                    return;
                _fontSizePreview = value;
                OnPropertyChanged();
            }
        }

        private bool CanExecuteSaveSettings =>
            SettingsChanged;

        public bool StyleSectionIsVisible =>
            DeviceSupportsManualDarkMode;
        #endregion

        #region Commands
        public ICommand SaveSettingsCommand { get; }
        #endregion

        #region Constructors
        public SettingsViewModel(
            Settings settings,
            Theming theming,
            ICommandFactoryService commandFactoryService,
            IMessagingService messagingService,
            IUiThreadService uiThreadService)
        {
            _settings = settings;
            _theming = theming;

            _subtitleColorPreview = _settings.GetSubtitleColor();
            _fontSizePreview = _settings.GetFontSize();

            _messagingService = messagingService;

            SaveSettingsCommand = commandFactoryService.Create(async () => await SaveSettingsAsync(), () => CanExecuteSaveSettings);

            #region History settings
            _currentHistoryLength = _settings.GetPlaybackHistoryLength();
            HistoryLength = _currentHistoryLength.ToString();
            #endregion History settings

            #region Theme settings
            DeviceSupportsManualDarkMode = _theming.DeviceSupportsManualDarkMode;
            var deviceSupportsAutomaticDarkMode = _theming.DeviceSupportsAutomaticDarkMode;
            _currentTheme = _theming.GetAppOrDefaultTheme();

            if (deviceSupportsAutomaticDarkMode)
                _themesDictionary.Add(LocalizedStrings.Device, null);

            uiThreadService.ExecuteOnUiThread(() =>
            {
                Themes = _themesDictionary.Keys.ToArray();
                SelectedTheme = _themesDictionary.FirstOrDefault(pair => pair.Value == _currentTheme).Key;
            });
            #endregion Theme settings

            #region Subtitle color settings
            _currentSubtitleColor = _settings.GetSubtitleColor();
            uiThreadService.ExecuteOnUiThread(() =>
            {
                SubtitleColors = _subtitleColorsDictionary.Keys.ToArray();
                SelectedSubtitleColor = _subtitleColorsDictionary.FirstOrDefault(pair => pair.Value == _currentSubtitleColor).Key;
            });
            #endregion Subtitle color settings

            #region Font size settings
            _currentFontSize = _settings.GetFontSize();
            FontSize = _currentFontSize.ToString();
            #endregion Font size settings

            #region Offset settings
            _currentOffset = _settings.GetOffset();
            Offset = _currentOffset.ToString();
            #endregion Offset settings

            uiThreadService.ExecuteOnUiThread(() => _loaded = true);
        }
        #endregion

        #region Methods
        private async Task SaveSettingsAsync()
        {
            await ManageHistoryLengthSettingsAsync();
            await ManageThemeSettingsAsync();
            ManageSubtitleColorSettings();
            ManageFontSizeSettings();
            ManageOffsetSettings();
            SettingsChanged = false;
            _messagingService.Send(this, Strings.SettingsChanged);
        }

        private async Task ManageHistoryLengthSettingsAsync()
        {
            // Try to use the value that the user inputted, else, use the configuration default
            if (!int.TryParse(HistoryLength, out var historyLengthAsInt))
            {
                historyLengthAsInt = Numbers.HistoryLengthDefault;
                HistoryLength = historyLengthAsInt.ToString();
            }
            if (_currentHistoryLength == historyLengthAsInt)
                return;
            _settings.SetPlaybackHistoryLength(historyLengthAsInt);
            // Clear storage for out of bounds results
            var newHistoryLengthIsZero = historyLengthAsInt == 0;
            if (historyLengthAsInt - _currentHistoryLength < 0
                && _settings.ContainsPlaybackHistory()
                && !newHistoryLengthIsZero)
            {
                var resultsHistory = await _settings.GetPlaybackHistoryAsync();
                if (historyLengthAsInt < resultsHistory.Count)
                {
                    resultsHistory.Reverse();
                    resultsHistory = resultsHistory.Take(historyLengthAsInt).ToList();
                    resultsHistory.Reverse();
                    _settings.SetPlaybackHistoryAsync(resultsHistory).AwaitInOtherContext(true);
                }
            }
            else if (newHistoryLengthIsZero)
                _settings.ClearPlaybackHistory();
            _currentHistoryLength = historyLengthAsInt;
        }

        private async Task ManageThemeSettingsAsync()
        {
            var selectedTheme = _themesDictionary[SelectedTheme];
            if (_currentTheme == selectedTheme)
                return;
            if (selectedTheme.HasValue)
                _settings.SetTheme(selectedTheme.Value);
            else
                _settings.ClearTheme();
            await _theming.ManageAppThemeAsync();
            _currentTheme = selectedTheme;
        }

        private void ManageSubtitleColorSettings()
        {
            var selectedSubtitleColor = _subtitleColorsDictionary[SelectedSubtitleColor];
            if (_currentSubtitleColor == selectedSubtitleColor)
                return;
            _settings.SetSubtitleColor(selectedSubtitleColor);
            _currentSubtitleColor = selectedSubtitleColor;
        }

        private void ManageFontSizeSettings()
        {
            // Try to use the value that the user inputted, else, use the configuration default
            if (!int.TryParse(FontSize, out var fontSizeAsInt))
            {
                fontSizeAsInt = Numbers.FontSizeDefault;
                FontSize = fontSizeAsInt.ToString();
            }
            if (_currentFontSize == fontSizeAsInt)
                return;
            _settings.SetFontSize(fontSizeAsInt);
            _currentFontSize = fontSizeAsInt;
        }

        private void ManageOffsetSettings()
        {
            // Try to use the value that the user inputted, else, use the configuration default
            if (!int.TryParse(Offset, out var fontSizeAsInt))
            {
                fontSizeAsInt = Numbers.OffsetDefault;
                Offset = fontSizeAsInt.ToString();
            }
            if (_currentOffset == fontSizeAsInt)
                return;
            _settings.SetOffset(fontSizeAsInt);
            _currentOffset = fontSizeAsInt;
        }
        #endregion
    }
}
