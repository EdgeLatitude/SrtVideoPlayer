using Foundation;
using SrtVideoPlayer.Mobile.DependencyServices;
using SrtVideoPlayer.Mobile.iOS.DependencyServices;
using System;
using System.IO;
using System.Linq;
using System.Text;
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

        private async void OnDocumentPickerDidPickDocumentAtUrls(object sender, UIDocumentPickedAtUrlsEventArgs args)
        {
            if (args.Urls.Any())
            {
                var url = args.Urls[0];
                url.StartAccessingSecurityScopedResource();
                _taskCompletionSource.SetResult(await ReadContentFromUrl(url));
                url.StopAccessingSecurityScopedResource();
            }
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

        private async Task<string> ReadContentFromUrl(NSUrl url)
        {
            using var stream = new FileStream(url.RelativePath, FileMode.Open, FileAccess.Read);
            using var streamReader = new StreamReader(stream, Encoding.UTF8);
            return await streamReader.ReadToEndAsync();
        }
    }
}
