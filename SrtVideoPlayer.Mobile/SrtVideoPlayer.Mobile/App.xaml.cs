using MediaManager;
using MediaManager.Video;
using SrtVideoPlayer.Mobile.Pages;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Files;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile
{
    public partial class App : Application
    {
        public App(VideoFile videoFile = null)
        {
            InitializeComponent();
            ViewModelLocator.Initialize();
            MainPage = new NavigationPage(new PlayerPage(videoFile));
            // MediaPlayer settings.
            CrossMediaManager.Current.KeepScreenOn = true;
            CrossMediaManager.Current.MediaPlayer.VideoAspect = VideoAspectMode.AspectFit;
        }

        protected override void OnStart()
        {
            base.OnStart();
            Theming.Instance.ManageAppTheme(true);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Theming.Instance.ManageAppTheme();
        }
    }
}
