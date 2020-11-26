using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IFileDownloaderService
    {
        Task<string> DownloadFileToCacheAsync(string url, string name, string extension, bool overwriteFile);
    }
}
