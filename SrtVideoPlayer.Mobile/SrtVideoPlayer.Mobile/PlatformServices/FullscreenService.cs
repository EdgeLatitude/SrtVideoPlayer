using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Linq;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class FullscreenService : IFullscreenService
    {
        private static readonly IFullscreenDependencyService _fullscreenDependencyService
            = DependencyService.Get<IFullscreenDependencyService>();

        public void Disable()
        {
            NavigationPage.SetHasNavigationBar(Application.Current.MainPage.Navigation.NavigationStack.Last(), true);
            _fullscreenDependencyService.Disable();
        }

        public void Enable(bool landscapeMode)
        {
            NavigationPage.SetHasNavigationBar(Application.Current.MainPage.Navigation.NavigationStack.Last(), false);
            _fullscreenDependencyService.Enable(landscapeMode);
        }
    }
}
