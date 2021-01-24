using SrtVideoPlayer.Mobile.Pages;
using SrtVideoPlayer.Shared.Logic;
using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            SharedInitialization();
            MainPage = new NavigationPage(new PlayerPage());
        }

        public App(string videoUri)
        {
            SharedInitialization();
            MainPage = new NavigationPage(new PlayerPage(videoUri));
        }

        private void SharedInitialization()
        {
            ViewModelLocator.Initialize();
            InitializeComponent();
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
