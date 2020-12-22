using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Mobile.iOS.DependencyServices;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SubtitlesPickerService))]
namespace SrtVideoPlayer.Mobile.iOS.DependencyServices
{
    public class SubtitlesPickerService : ISubtitlesPickerService
    {
        private const string _subtitlesMediaType = "public.data";

        private readonly string[] _documentTypes = {
            _subtitlesMediaType
        };

        private UIDocumentPickerViewController _documentPicker;
        private TaskCompletionSource<string> _taskCompletionSource;

        public Task<string> ReadSubtitlesAsync()
        {
            _documentPicker = new UIDocumentPickerViewController(_documentTypes, UIDocumentPickerMode.Open);

            _documentPicker.DidPickDocumentAtUrls += OnDocumentPickerDidPickDocumentAtUrls;
            _documentPicker.WasCancelled += OnDocumentPickerWasCancelled;

            var window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentViewController(_documentPicker, true, null);

            _taskCompletionSource = new TaskCompletionSource<string>();
            return _taskCompletionSource.Task;
        }

        private void OnDocumentPickerDidPickDocumentAtUrls(object sender, UIDocumentPickedAtUrlsEventArgs args)
        {
            if (args.Urls.Any())
                _taskCompletionSource.SetResult(args.Urls[0].AbsoluteString);
            else
                _taskCompletionSource.SetResult(null);
            _documentPicker.DismissModalViewController(true);
            DetachHandlers();
        }

        private void OnDocumentPickerWasCancelled(object sender, EventArgs args)
        {
            _taskCompletionSource.SetResult(null);
            _documentPicker.DismissModalViewController(true);
            DetachHandlers();
        }

        private void DetachHandlers()
        {
            _documentPicker.DidPickDocumentAtUrls -= OnDocumentPickerDidPickDocumentAtUrls;
            _documentPicker.WasCancelled -= OnDocumentPickerWasCancelled;
        }
    }
}
