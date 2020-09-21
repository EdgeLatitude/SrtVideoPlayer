using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Mobile.iOS.Services;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoPickerService))]
namespace SrtVideoPlayer.Mobile.iOS.Services
{
    public class VideoPickerService : IVideoPickerService
    {
        private const string _videoMediaType = "public.movie";

        private UIImagePickerController _videoPicker;
        private TaskCompletionSource<string> _taskCompletionSource;

        public Task<string> GetVideoAsync()
        {
            _videoPicker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum,
                MediaTypes = new string[] { _videoMediaType }
            };

            _videoPicker.FinishedPickingMedia += OnVideoPickerFinishedPickingMedia;
            _videoPicker.Canceled += OnVideoPickerCancelled;

            var window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentViewController(_videoPicker, true, null);

            _taskCompletionSource = new TaskCompletionSource<string>();
            return _taskCompletionSource.Task;
        }

        private void OnVideoPickerFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs args)
        {
            if (args.MediaType == _videoMediaType)
                _taskCompletionSource.SetResult(args.MediaUrl.AbsoluteString);
            else
                _taskCompletionSource.SetResult(null);
            _videoPicker.DismissModalViewController(true);
            DetachHandlers();
        }

        private void OnVideoPickerCancelled(object sender, EventArgs args)
        {
            _taskCompletionSource.SetResult(null);
            _videoPicker.DismissModalViewController(true);
            DetachHandlers();
        }

        private void DetachHandlers()
        {
            _videoPicker.FinishedPickingMedia -= OnVideoPickerFinishedPickingMedia;
            _videoPicker.Canceled -= OnVideoPickerCancelled;
        }
    }
}
