using SrtVideoPlayer.Shared.Models.Files;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IFilePickerService
    {
        Task<VideoFile> SelectVideoAsync();
        Task<SubtitlesFile> SelectSubtitlesAsync();
    }
}
