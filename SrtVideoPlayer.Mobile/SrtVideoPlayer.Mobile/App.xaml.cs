using MediaManager;
using MediaManager.Video;
using SrtVideoPlayer.Mobile.Pages;
using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Files;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SrtVideoPlayer.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {
        private readonly Theming _theming;

        public App(VideoFile videoFile = null)
        {
            InitializeComponent();
            ViewModelLocator.Initialize();
            var playerPage = (PlayerPage)ViewModelLocator.Instance.ResolvePage(Locations.PlayerPage);
            playerPage.VideoFile = videoFile;
            MainPage = new NavigationPage(playerPage);
            _theming = ViewModelLocator.Instance.Resolve<Theming>();
            // MediaPlayer settings.
            CrossMediaManager.Current.KeepScreenOn = true;
            CrossMediaManager.Current.MediaPlayer.VideoAspect = VideoAspectMode.AspectFit;
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await _theming.ManageAppThemeAsync(true);
        }

        protected override async void OnResume()
        {
            base.OnResume();
            await _theming.ManageAppThemeAsync();
        }
    }
}
