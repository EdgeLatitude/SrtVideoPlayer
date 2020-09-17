using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class FilePickerService : IFilePickerService
    {
        public async Task<string> SelectVideoAsync() =>
            await DependencyService.Get<IVideoPickerService>().GetVideoAsync();

        public async Task<string> SelectSubtitlesAsync() =>
            await DependencyService.Get<ISubtitlesPickerService>().GetSubtitlesAsync();
    }
}
