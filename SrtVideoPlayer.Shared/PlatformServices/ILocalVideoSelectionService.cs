using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface ILocalVideoSelectionService
    {
        Task<string> SelectVideoAsync();
    }
}
