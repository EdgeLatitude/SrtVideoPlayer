using SrtVideoPlayer.Mobile.Pages;
using SrtVideoPlayer.Shared.Logic;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            ViewModelLocator.Initialize();
            InitializeComponent();
            MainPage = new NavigationPage(new SrtVideoPlayerPage());
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
