using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Shared.PlatformServices;
using SrtVideoPlayer.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class FullscreenService : IFullscreenService
    {
        private readonly IFullscreenDependencyService _fullscreenDependencyService
            = DependencyService.Get<IFullscreenDependencyService>();

        private readonly IDictionary<Xamarin.Forms.Page, ToolbarItem[]> _originalToolbarItems
            = new Dictionary<Xamarin.Forms.Page, ToolbarItem[]>();

        public void Disable(BaseViewModel callerViewModel)
        {
            _fullscreenDependencyService.Disable();
            var correspondingPages = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack
                .Where(page => page.BindingContext == callerViewModel).ToArray();
            foreach (var correspondingPage in correspondingPages)
            {
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(correspondingPage, true);
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (_originalToolbarItems.ContainsKey(correspondingPage))
                    {
                        var originalToolbarItems = _originalToolbarItems[correspondingPage];
                        foreach (var originalToolbarItem in originalToolbarItems)
                            correspondingPage.ToolbarItems.Add(originalToolbarItem);
                        _originalToolbarItems.Remove(correspondingPage);
                    }
                    correspondingPage.On<iOS>().SetPrefersHomeIndicatorAutoHidden(false);
                    correspondingPage.On<iOS>().SetUseSafeArea(true);
                }
            }
        }

        public void Enable(BaseViewModel callerViewModel, bool landscapeMode)
        {
            _fullscreenDependencyService.Enable(landscapeMode);
            var correspondingPages = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack
                .Where(page => page.BindingContext == callerViewModel).ToArray();
            foreach (var correspondingPage in correspondingPages)
            {
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(correspondingPage, false);
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (!_originalToolbarItems.ContainsKey(correspondingPage))
                    {
                        _originalToolbarItems.Add(correspondingPage, correspondingPage.ToolbarItems.ToArray());
                        correspondingPage.ToolbarItems.Clear();
                    }
                    correspondingPage.On<iOS>().SetPrefersHomeIndicatorAutoHidden(true);
                    var lastPadding = correspondingPage.Padding;
                    correspondingPage.On<iOS>().SetUseSafeArea(false);
                    if (landscapeMode)
                        correspondingPage.Padding = new Thickness(lastPadding.Left, 0, lastPadding.Right, 0);
                    else
                        correspondingPage.Padding = new Thickness(0, lastPadding.Top, 0, lastPadding.Bottom);
                }
            }
        }
    }
}
