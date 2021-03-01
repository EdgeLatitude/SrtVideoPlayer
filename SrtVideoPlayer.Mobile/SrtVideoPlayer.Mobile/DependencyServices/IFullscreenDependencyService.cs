namespace SrtVideoPlayer.Mobile.DependencyServices
{
    public interface IFullscreenDependencyService
    {
        void Disable();
        void Enable(bool landscapeMode);
    }
}
