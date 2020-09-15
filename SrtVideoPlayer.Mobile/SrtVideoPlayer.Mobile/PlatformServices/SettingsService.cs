using SrtVideoPlayer.Shared.PlatformServices;
using System;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class SettingsService : ISettingsService
    {
        public bool Get(string key, bool defaultValue) =>
            Preferences.Get(key, defaultValue);

        public void Set(string key, bool value) =>
            Preferences.Set(key, value);

        public int Get(string key, int defaultValue) =>
            Preferences.Get(key, defaultValue);

        public void Set(string key, int value) =>
            Preferences.Set(key, value);

        public double Get(string key, double defaultValue) =>
            Preferences.Get(key, defaultValue);

        public void Set(string key, double value) =>
            Preferences.Set(key, value);

        public string Get(string key, string defaultValue) =>
            Preferences.Get(key, defaultValue);

        public void Set(string key, string value) =>
            Preferences.Set(key, value);

        public DateTime Get(string key, DateTime defaultValue) =>
            Preferences.Get(key, defaultValue);

        public void Set(string key, DateTime value) =>
            Preferences.Set(key, value);

        public bool Contains(string key) =>
            Preferences.ContainsKey(key);

        public void Remove(string key) =>
            Preferences.Remove(key);

        public void Clear() =>
            Preferences.Clear();
    }
}
