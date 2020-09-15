using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface INavigationService
    {
        Task NavigateToAsync(string resource);
        Task NavigateBackAsync();
        Task NavigateBackToRootAsync();
    }
}
