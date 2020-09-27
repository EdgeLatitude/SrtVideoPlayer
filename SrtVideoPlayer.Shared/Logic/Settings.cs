﻿using Newtonsoft.Json;
using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.DataStructures;
using SrtVideoPlayer.Shared.Models.Playback;
using SrtVideoPlayer.Shared.Models.Theming;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Collections.Generic;
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

        public async Task ManageNewPlaybackAsync(History playback) =>
            await Task.Run(() => ManageNewPlayback(playback));

        private void ManageNewPlayback(History playback)
        {
            var playbackHistory = new CircularBuffer<History>(GetPlaybackHistoryLength(),
                ContainsPlaybackHistory() ? GetPlaybackHistory() : new List<History>());
            playbackHistory.Write(playback);
            SetPlaybackHistory(new CircularBuffer<History>(GetPlaybackHistoryLength(), playbackHistory));
        }

        public async Task<List<History>> GetPlaybackHistoryAsync() =>
            await Task.Run(GetPlaybackHistory);

        private List<History> GetPlaybackHistory() =>
            JsonConvert.DeserializeObject<List<History>>(_settingsService.Get(Strings.History, string.Empty));

        public async Task SetPlaybackHistoryAsync(IEnumerable<History> playbackHistory) =>
            await Task.Run(() => SetPlaybackHistory(playbackHistory));

        private void SetPlaybackHistory(IEnumerable<History> playbackHistory) =>
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
    }
}
