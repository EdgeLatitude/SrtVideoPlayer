using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Theming;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace SrtVideoPlayer.Shared.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ICommandFactoryService _commandFactoryService;
        private readonly IUiThreadService _uiThreadService;

        private readonly bool _deviceSupportsAutomaticDarkMode;

        private readonly Dictionary<string, Theme?> _themesDictionary = new Dictionary<string, Theme?>
        {
            { LocalizedStrings.Light, Theme.Light },
            { LocalizedStrings.Dark, Theme.Dark }
        };

        private readonly Dictionary<string, string> _subtitleColorsDictionary = new Dictionary<string, string>
        {
            { LocalizedStrings.White, Colors.White },
            { LocalizedStrings.Yellow, Colors.Yellow },
            { LocalizedStrings.Cyan, Colors.Cyan }
        };

        private bool _loaded;

        private int _currentHistoryLength;

        private Theme? _currentTheme;

        private string _currentSubtitleColor;

        public SettingsViewModel(
            ICommandFactoryService commandFactoryService,
            IUiThreadService uiThreadService)
        {
            _commandFactoryService = commandFactoryService;
            _uiThreadService = uiThreadService;

            SaveSettingsCommand = _commandFactoryService.Create(() => SaveSettings(), CanExecuteSaveSettings);

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
            _currentSubtitleColor = Settings.Instance.GetSubtitleColorLength();
            _uiThreadService.ExecuteOnUiThread(() =>
            {
                SubtitleColors = _subtitleColorsDictionary.Keys.ToArray();
                SelectedSubtitleColor = _subtitleColorsDictionary.FirstOrDefault(pair => pair.Value == _currentSubtitleColor).Key;
            });
            #endregion Subtitle color settings

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
                    && !value.All(c => char.IsNumber(c)))
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

        public bool StyleSectionIsVisible => DeviceSupportsManualDarkMode;

        public ICommand SaveSettingsCommand { get; }

        private void SaveSettings()
        {
            ManageHistoryLengthSettings();
            ManageThemeSettings();
            SettingsChanged = false;
        }

        private async void ManageHistoryLengthSettings()
        {
            // Try to use the value that the user inputted, else, use the configuration default
            if (!int.TryParse(HistoryLength, out int historyLengthAsInt))
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

        private bool CanExecuteSaveSettings() =>
            SettingsChanged;
    }
}
