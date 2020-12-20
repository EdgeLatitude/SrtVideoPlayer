using System.Threading.Tasks;

namespace SrtVideoPlayer.Mobile.DependencyServices
{
    public interface IVideoPickerService
    {
        Task<string> SelectVideoAsync();
    }
}
