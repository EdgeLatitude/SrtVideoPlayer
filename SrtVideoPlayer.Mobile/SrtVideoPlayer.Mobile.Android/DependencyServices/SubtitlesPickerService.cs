using Android.Content;
using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Mobile.Droid.Services;
using SrtVideoPlayer.Shared.Localization;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SubtitlesPickerService))]
namespace SrtVideoPlayer.Mobile.Droid.Services
{
    public class SubtitlesPickerService : ISubtitlesPickerService
    {
        private const string _subtitlesMediaType = "*/*";

        public Task<string> ReadSubtitlesAsync()
        {
            var intent = new Intent();
            intent.SetType(_subtitlesMediaType);
            intent.SetAction(Intent.ActionGetContent);

            var activity = MainActivity.Instance;
            activity.StartActivityForResult(Intent.CreateChooser(intent, LocalizedStrings.SelectSubtitles), MainActivity.ReadSubtitlesId);
            activity.PickSubtitlesTaskCompletionSource = new TaskCompletionSource<string>();

            return activity.PickSubtitlesTaskCompletionSource.Task;
        }
    }
}
