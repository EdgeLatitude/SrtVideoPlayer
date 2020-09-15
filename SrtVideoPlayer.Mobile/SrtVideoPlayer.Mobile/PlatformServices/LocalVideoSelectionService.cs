using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class LocalVideoSelectionService : ILocalVideoSelectionService
    {
        public async Task<string> SelectVideoAsync() =>
            await DependencyService.Get<IVideoPickerService>().GetVideoAsync();
    }
}
