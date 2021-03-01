using SrtVideoPlayer.Mobile.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(SrtVideoPlayer.Mobile.iOS.DependencyServices.FullscreenDependencyService))]
namespace SrtVideoPlayer.Mobile.iOS.DependencyServices
{
    public class FullscreenDependencyService : IFullscreenDependencyService
    {
        public void Disable()
        {
            throw new System.NotImplementedException();
        }

        public void Enable(bool landscapeMode)
        {
            throw new System.NotImplementedException();
        }
    }
}
