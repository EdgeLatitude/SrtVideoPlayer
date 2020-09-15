using System;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface ISettingsService
    {
        bool Get(string key, bool defaultValue);
        void Set(string key, bool value);
        int Get(string key, int defaultValue);
        void Set(string key, int value);
        double Get(string key, double defaultValue);
        void Set(string key, double value);
        string Get(string key, string defaultValue);
        void Set(string key, string value);
        DateTime Get(string key, DateTime defaultValue);
        void Set(string key, DateTime value);
        bool Contains(string key);
        void Remove(string key);
        void Clear();
    }
}
