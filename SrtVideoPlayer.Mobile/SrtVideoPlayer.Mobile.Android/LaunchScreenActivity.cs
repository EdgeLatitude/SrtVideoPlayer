using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Mobile.Droid
{
    [IntentFilter(
        new string[] { Intent.ActionView },
        Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataHost = "*",
        DataMimeType = "video/*",
        DataScheme = "content")]
    [Activity(
        Icon = "@mipmap/icon",
        Label = "@string/short_app_name",
        MainLauncher = true,
        NoHistory = true,
        Theme = "@style/MainTheme.Splash")]
    public class LaunchScreenActivity : AppCompatActivity
    {
        private bool _mainActivityCreationStarted;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_mainActivityCreationStarted)
                return;
            new Task(() =>
            {
                using var mainActivityIntent = new Intent(Application.Context, typeof(MainActivity));
                mainActivityIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                mainActivityIntent.AddFlags(ActivityFlags.NoAnimation);
                var intentData = Intent?.Data;
                if (intentData != null)
                    mainActivityIntent.SetData(intentData);
                StartActivity(mainActivityIntent);
            }).Start();
            _mainActivityCreationStarted = true;
        }

        public override void OnBackPressed() { }
    }
}
