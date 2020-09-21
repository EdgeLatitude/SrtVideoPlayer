using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IFileDownloaderService
    {
        Task<string> DownloadFileAsync(string url, string name, string extension, bool overwriteFile);
    }
}
