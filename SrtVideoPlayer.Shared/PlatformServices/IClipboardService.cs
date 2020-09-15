using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IClipboardService
    {
        Task SetTextAsync(string text);
    }
}
