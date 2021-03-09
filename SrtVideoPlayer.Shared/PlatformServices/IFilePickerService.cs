using SrtVideoPlayer.Shared.Models.Files;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IFilePickerService
    {
        Task<VideoFile> SelectVideoFromGalleryAsync();
        Task<VideoFile> SelectVideoFromFilesAsync();
        Task<SubtitlesFile> SelectSubtitlesAsync();
    }
}
