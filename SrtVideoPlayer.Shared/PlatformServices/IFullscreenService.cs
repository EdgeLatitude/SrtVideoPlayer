using SrtVideoPlayer.Shared.ViewModels;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IFullscreenService
    {
        void Disable(BaseViewModel callerViewModel);
        void Enable(BaseViewModel callerViewModel, bool landscapeMode);
    }
}
