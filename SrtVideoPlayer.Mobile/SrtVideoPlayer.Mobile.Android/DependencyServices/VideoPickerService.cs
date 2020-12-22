using Android.Content;
using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Mobile.Droid.DependencyServices;
using SrtVideoPlayer.Shared.Localization;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoPickerService))]
namespace SrtVideoPlayer.Mobile.Droid.DependencyServices
{
    public class VideoPickerService : IVideoPickerService
    {
        private const string _videoMediaType = "video/*";

        public Task<string> SelectVideoAsync()
        {
            var intent = new Intent();
            intent.SetType(_videoMediaType);
            intent.SetAction(Intent.ActionGetContent);

            var activity = MainActivity.Instance;
            activity.StartActivityForResult(Intent.CreateChooser(intent, LocalizedStrings.SelectVideo), MainActivity.SelectVideoId);
            activity.PickVideoTaskCompletionSource = new TaskCompletionSource<string>();

            return activity.PickVideoTaskCompletionSource.Task;
        }
    }
}
