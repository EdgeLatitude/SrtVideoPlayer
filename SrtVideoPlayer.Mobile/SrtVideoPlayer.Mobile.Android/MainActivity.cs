using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using SrtVideoPlayer.Shared.Models.Theming;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Mobile.Droid
{
    [Activity(Theme = "@style/LaunchTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const int PickVideoId = 1000;

        public const int PickSubtitlesId = 1001;

        public static MainActivity Instance { get; private set; }

        public TaskCompletionSource<string> PickVideoTaskCompletionSource { get; set; }

        public TaskCompletionSource<string> PickSubtitlesTaskCompletionSource { get; set; }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            LoadApplication(new App());

            Shared.Logic.Theming.Instance.ThemeChangeNeeded += GlobalEvents_ThemeChangeNeeded;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == PickVideoId)
                if ((resultCode == Result.Ok) && (data != null))
                    PickVideoTaskCompletionSource.SetResult(data.DataString);
                else
                    PickVideoTaskCompletionSource.SetResult(null);
        }

        private void GlobalEvents_ThemeChangeNeeded(object sender, ThemeChangeNeededEventArgs args)
        {
            switch (args.Theme)
            {
                case Shared.Models.Theming.Theme.Dark:
                    SetTheme(Resource.Style.MainTheme);
                    break;
                case Shared.Models.Theming.Theme.Light:
                    SetTheme(Resource.Style.MainTheme_Light);
                    break;
            }
        }
    }
}
