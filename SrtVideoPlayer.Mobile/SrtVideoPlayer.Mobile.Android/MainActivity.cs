using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Widget;
using AndroidX.DocumentFile.Provider;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Models.Files;
using SrtVideoPlayer.Shared.Models.Theming;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Mobile.Droid
{
    [Activity(
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
        LaunchMode = LaunchMode.SingleTop,
        Theme = "@style/LaunchTheme")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const int _readExternalStorageRequestCode = 1;

        public const int SelectVideoId = 1000;

        public const int ReadSubtitlesId = 1001;

        public TaskCompletionSource<string> PickVideoTaskCompletionSource { get; set; }

        public TaskCompletionSource<string> PickSubtitlesTaskCompletionSource { get; set; }

        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            MediaManager.CrossMediaManager.Current.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            UserDialogs.Init(this);

            if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted)
                RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, _readExternalStorageRequestCode);

            var intentData = Intent?.Data;
            if (intentData == null)
                LoadApplication(new App());
            else
            {
                var documentFile = DocumentFile.FromSingleUri(ApplicationContext, intentData);
                LoadApplication(new App(new VideoFile
                {
                    Name = documentFile.Name,
                    Path = documentFile.Uri.ToString()
                }));
            }

            Shared.Logic.Theming.ThemeChangeNeeded += GlobalEvents_ThemeChangeNeeded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Shared.Logic.Theming.ThemeChangeNeeded -= GlobalEvents_ThemeChangeNeeded;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (requestCode)
            {
                case SelectVideoId:
                    if (resultCode == Result.Ok && data != null)
                        PickVideoTaskCompletionSource.SetResult(data.DataString);
                    else
                        PickVideoTaskCompletionSource.SetResult(null);
                    break;
                case ReadSubtitlesId:
                    if (resultCode == Result.Ok && data != null)
                        PickSubtitlesTaskCompletionSource.SetResult(await ReadContentFromContentUriAsync(data.Data));
                    else
                        PickSubtitlesTaskCompletionSource.SetResult(null);
                    break;
            }
        }

        private async Task<string> ReadContentFromContentUriAsync(Uri contentUri)
        {
            try
            {
                using var stream = ContentResolver.OpenInputStream(contentUri);
                using var streamReader = new StreamReader(stream, Encoding.UTF8);
                return await streamReader.ReadToEndAsync();
            }
            catch (System.Exception)
            {
                Toast.MakeText(this, LocalizedStrings.InvalidSubtitlesFile, ToastLength.Long).Show();
                return null;
            }
        }
    }
}
