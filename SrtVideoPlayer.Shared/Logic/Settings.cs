using Newtonsoft.Json;
using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.DataStructures;
using SrtVideoPlayer.Shared.Models.Playback;
using SrtVideoPlayer.Shared.Models.Theming;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.Logic
{
    public class Settings
    {
        private static ISettingsService _settingsService;
        private static IThemingService _themingService;

        public static Settings Instance
        {
            get;
            private set;
        }

        public static void Initialize(
            ISettingsService settingsService,
            IThemingService themingService) =>
            Instance = new Settings(settingsService, themingService);

        private Settings(
            ISettingsService settingsService,
            IThemingService themingService)
        {
            _settingsService = settingsService;
            _themingService = themingService;
        }

        public int GetPlaybackHistoryLength() =>
            _settingsService.Get(Strings.HistoryLength, Numbers.HistoryLengthDefault);

        public void SetPlaybackHistoryLength(int length) =>
            _settingsService.Set(Strings.HistoryLength, length);

        public bool ContainsPlaybackHistory() =>
            _settingsService.Contains(Strings.History);

        public void ClearPlaybackHistory() =>
            _settingsService.Remove(Strings.History);

        public async Task ManageNewPlaybackAsync(Playback playback) =>
            await Task.Run(() => ManageNewPlayback(playback));

        private void ManageNewPlayback(Playback playback)
        {
            var playbackHistory = new CircularBuffer<Playback>(GetPlaybackHistoryLength(),
                ContainsPlaybackHistory() ? GetPlaybackHistory() : new List<Playback>());

            var playbackWithTheSameVideoName = playbackHistory.FirstOrDefault(innerPlayback => innerPlayback.Video.Name == playback.Video.Name);
            if (playbackWithTheSameVideoName != null)
            {
                var newPlaybackHistory = new CircularBuffer<Playback>(GetPlaybackHistoryLength());
                while (playbackHistory.Any())
                    if (playbackHistory.Peek() != playbackWithTheSameVideoName)
                        newPlaybackHistory.Enqueue(playbackHistory.Dequeue());
                    else
                        playbackHistory.Dequeue();
                newPlaybackHistory.Enqueue(playbackWithTheSameVideoName);
                playbackHistory = newPlaybackHistory;
            }
            else
                playbackHistory.Enqueue(playback);

            SetPlaybackHistory(new CircularBuffer<Playback>(GetPlaybackHistoryLength(), playbackHistory));
        }

        public async Task<List<Playback>> GetPlaybackHistoryAsync() =>
            await Task.Run(GetPlaybackHistory);

        private List<Playback> GetPlaybackHistory() =>
            JsonConvert.DeserializeObject<List<Playback>>(_settingsService.Get(Strings.History, string.Empty));

        public async Task SetPlaybackHistoryAsync(IEnumerable<Playback> playbackHistory) =>
            await Task.Run(() => SetPlaybackHistory(playbackHistory));

        private void SetPlaybackHistory(IEnumerable<Playback> playbackHistory) =>
            _settingsService.Set(Strings.History, JsonConvert.SerializeObject(playbackHistory,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));

        public Theme GetTheme() =>
            (Theme)Enum.Parse(typeof(Theme),
                _settingsService.Get(Strings.Theme, _themingService.GetDeviceDefaultTheme().ToString()));

        public void SetTheme(Theme theme) =>
            _settingsService.Set(Strings.Theme, theme.ToString());

        public bool ContainsTheme() =>
            _settingsService.Contains(Strings.Theme);

        public void ClearTheme() =>
            _settingsService.Remove(Strings.Theme);

        public string GetSubtitleColor() =>
            _settingsService.Get(Strings.SubtitleColor, Strings.SubtitleColorDefault);

        public void SetSubtitleColor(string color) =>
            _settingsService.Set(Strings.SubtitleColor, color);

        public int GetFontSize() =>
            _settingsService.Get(Strings.FontSize, Numbers.FontSizeDefault);

        public void SetFontSize(int size) =>
            _settingsService.Set(Strings.FontSize, size);

        public int GetOffset() =>
            _settingsService.Get(Strings.Offset, Numbers.OffsetDefault);

        public void SetOffset(int size) =>
            _settingsService.Set(Strings.Offset, size);
    }
}
