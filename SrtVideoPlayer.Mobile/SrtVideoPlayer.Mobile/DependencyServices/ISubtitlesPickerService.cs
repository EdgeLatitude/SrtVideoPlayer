using System.Threading.Tasks;

namespace SrtVideoPlayer.Mobile.DependencyServices
{
    public interface ISubtitlesPickerService
    {
        Task<string> GetSubtitlesAsync();
    }
}
