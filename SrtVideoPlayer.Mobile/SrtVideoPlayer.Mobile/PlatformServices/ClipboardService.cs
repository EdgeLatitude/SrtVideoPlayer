using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class ClipboardService : IClipboardService
    {
        public Task SetTextAsync(string text) =>
            Clipboard.SetTextAsync(text);
    }
}
