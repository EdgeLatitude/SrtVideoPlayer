using Acr.UserDialogs;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class FileDownloaderService : IFileDownloaderService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly IAlertsService _alertsService;

        public FileDownloaderService(IAlertsService alertsService)
        {
            _alertsService = alertsService;
        }

        public async Task<string> DownloadFileToCacheAsync(string url, string name, string extension, bool overwriteFile)
        {
            byte[] fileBytes = null;
            try
            {
                using (UserDialogs.Instance.Loading(LocalizedStrings.DownloadingFile))
                    fileBytes = await _httpClient.GetByteArrayAsync(url);
            }
            catch (Exception exception)
            {
                await _alertsService.DisplayAlertAsync(LocalizedStrings.Error, $"{LocalizedStrings.FileDownloadError} {exception.Message}");
            }

            if (fileBytes == null)
                return null;

            var filepath = Path.Combine(FileSystem.CacheDirectory, $"{name}{extension}");
            if (File.Exists(filepath)
                && overwriteFile)
                File.Delete(filepath);
            if (!File.Exists(filepath))
                await File.WriteAllBytesAsync(filepath, fileBytes);
            return filepath;
        }
    }
}
