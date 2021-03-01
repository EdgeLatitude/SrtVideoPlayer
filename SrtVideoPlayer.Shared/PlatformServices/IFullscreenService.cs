namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IFullscreenService
    {
        void Disable();
        void Enable(bool landscapeMode);
    }
}
