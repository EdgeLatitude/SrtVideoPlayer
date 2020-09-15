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

        private UIImagePickerController _imagePicker;
        private TaskCompletionSource<string> _taskCompletionSource;

        public Task<string> GetVideoAsync()
        {
            _imagePicker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum,
                MediaTypes = new string[] { _videoMediaType }
            };

            _imagePicker.FinishedPickingMedia += OnImagePickerFinishedPickingMedia;
            _imagePicker.Canceled += OnImagePickerCancelled;

            var window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentViewController(_imagePicker, true, null);

            _taskCompletionSource = new TaskCompletionSource<string>();
            return _taskCompletionSource.Task;
        }

        private void OnImagePickerFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs args)
        {
            if (args.MediaType == _videoMediaType)
                _taskCompletionSource.SetResult(args.MediaUrl.AbsoluteString);
            else
                _taskCompletionSource.SetResult(null);
            _imagePicker.DismissModalViewController(true);
            DetachHandlers();
        }

        private void OnImagePickerCancelled(object sender, EventArgs args)
        {
            _taskCompletionSource.SetResult(null);
            _imagePicker.DismissModalViewController(true);
            DetachHandlers();
        }

        private void DetachHandlers()
        {
            _imagePicker.FinishedPickingMedia -= OnImagePickerFinishedPickingMedia;
            _imagePicker.Canceled -= OnImagePickerCancelled;
        }
    }
}
