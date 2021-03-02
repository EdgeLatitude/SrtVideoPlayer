using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Theming;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SrtVideoPlayer.Shared.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private const string _minusCharacter = "-";

        private readonly ICommandFactoryService _commandFactoryService;
        private readonly IMessagingService _messagingService;
        private readonly IUiThreadService _uiThreadService;

        private readonly bool _deviceSupportsAutomaticDarkMode;

        private readonly Dictionary<string, Theme?> _themesDictionary = new Dictionary<string, Theme?>
        {
            { LocalizedStrings.Light, Theme.Light },
            { LocalizedStrings.Dark, Theme.Dark }
        };

        private readonly Dictionary<string, string> _subtitleColorsDictionary = new Dictionary<string, string>
        {
            { LocalizedStrings.Yellow, Colors.Yellow },
            { LocalizedStrings.White, Colors.White },
            { LocalizedStrings.Cyan, Colors.Cyan }
        };

        private bool _loaded;
        private int _currentHistoryLength;
        private Theme? _currentTheme;
        private string _currentSubtitleColor;
        private int _currentFontSize;
        private int _currentOffset;

        public SettingsViewModel(
            ICommandFactoryService commandFactoryService,
            IMessagingService messagingService,
            IUiThreadService uiThreadService)
        {
            _commandFactoryService = commandFactoryService;
            _messagingService = messagingService;
            _uiThreadService = uiThreadService;

            SaveSettingsCommand = _commandFactoryService.Create(SaveSettings, () => CanExecuteSaveSettings);

            #region History settings
            _currentHistoryLength = Settings.Instance.GetPlaybackHistoryLength();
            HistoryLength = _currentHistoryLength.ToString();
            #endregion History settings

            #region Theme settings
            DeviceSupportsManualDarkMode = Theming.Instance.DeviceSupportsManualDarkMode;
            _deviceSupportsAutomaticDarkMode = Theming.Instance.DeviceSupportsAutomaticDarkMode;
            _currentTheme = Theming.Instance.GetAppOrDefaultTheme();

            if (_deviceSupportsAutomaticDarkMode)
                _themesDictionary.Add(LocalizedStrings.Device, null);

            _uiThreadService.ExecuteOnUiThread(() =>
            {
                Themes = _themesDictionary.Keys.ToArray();
                SelectedTheme = _themesDictionary.FirstOrDefault(pair => pair.Value == _currentTheme).Key;
            });
            #endregion Theme settings

            #region Subtitle color settings
            _currentSubtitleColor = Settings.Instance.GetSubtitleColor();
            _uiThreadService.ExecuteOnUiThread(() =>
            {
                SubtitleColors = _subtitleColorsDictionary.Keys.ToArray();
                SelectedSubtitleColor = _subtitleColorsDictionary.FirstOrDefault(pair => pair.Value == _currentSubtitleColor).Key;
            });
            #endregion Subtitle color settings

            #region Font size settings
            _currentFontSize = Settings.Instance.GetFontSize();
            FontSize = _currentFontSize.ToString();
            #endregion Font size settings

            #region Offset settings
            _currentOffset = Settings.Instance.GetOffset();
            Offset = _currentOffset.ToString();
            #endregion Offset settings

            _uiThreadService.ExecuteOnUiThread(() => _loaded = true);
        }

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
                _selectedSubtitleColor = value;
                SettingsChanged = true;
                OnPropertyChanged();
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

        public bool StyleSectionIsVisible =>
            DeviceSupportsManualDarkMode;

        public ICommand SaveSettingsCommand { get; }

        private bool CanExecuteSaveSettings =>
            SettingsChanged;

        private void SaveSettings()
        {
            _ = ManageHistoryLengthSettings();
            ManageThemeSettings();
            ManageSubtitleColorSettings();
            ManageFontSizeSettings();
            ManageOffsetSettings();
            SettingsChanged = false;
            _messagingService.Send(this, Strings.SettingsChanged);
        }

        private async Task ManageHistoryLengthSettings()
        {
            // Try to use the value that the user inputted, else, use the configuration default
            if (!int.TryParse(HistoryLength, out var historyLengthAsInt))
            {
                historyLengthAsInt = Numbers.HistoryLengthDefault;
                HistoryLength = historyLengthAsInt.ToString();
            }
            if (_currentHistoryLength == historyLengthAsInt)
                return;
            Settings.Instance.SetPlaybackHistoryLength(historyLengthAsInt);
            // Clear storage for out of bounds results
            var newHistoryLengthIsZero = historyLengthAsInt == 0;
            if (historyLengthAsInt - _currentHistoryLength < 0
                && Settings.Instance.ContainsPlaybackHistory()
                && !newHistoryLengthIsZero)
            {
                var resultsHistory = await Settings.Instance.GetPlaybackHistoryAsync();
                if (historyLengthAsInt < resultsHistory.Count)
                {
                    resultsHistory.Reverse();
                    resultsHistory = resultsHistory.Take(historyLengthAsInt).ToList();
                    resultsHistory.Reverse();
                    _ = Settings.Instance.SetPlaybackHistoryAsync(resultsHistory);
                }
            }
            else if (newHistoryLengthIsZero)
                Settings.Instance.ClearPlaybackHistory();
            _currentHistoryLength = historyLengthAsInt;
        }

        private void ManageThemeSettings()
        {
            var selectedTheme = _themesDictionary[SelectedTheme];
            if (_currentTheme == selectedTheme)
                return;
            if (selectedTheme.HasValue)
                Settings.Instance.SetTheme(selectedTheme.Value);
            else
                Settings.Instance.ClearTheme();
            Theming.Instance.ManageAppTheme();
            _currentTheme = selectedTheme;
        }

        private void ManageSubtitleColorSettings()
        {
            var selectedSubtitleColor = _subtitleColorsDictionary[SelectedSubtitleColor];
            if (_currentSubtitleColor == selectedSubtitleColor)
                return;
            Settings.Instance.SetSubtitleColor(selectedSubtitleColor);
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
            Settings.Instance.SetFontSize(fontSizeAsInt);
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
            Settings.Instance.SetOffset(fontSizeAsInt);
            _currentOffset = fontSizeAsInt;
        }
    }
}
