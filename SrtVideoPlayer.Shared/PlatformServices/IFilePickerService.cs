using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IFilePickerService
    {
        Task<string> SelectVideoAsync();
        Task<string> ReadSubtitlesAsync();
    }
}
