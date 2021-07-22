using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Models.Files;
using SrtVideoPlayer.Shared.PlatformServices;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class FilePickerService : IFilePickerService
    {
        public async Task<VideoFile> SelectVideoFromGalleryAsync()
        {
            var file = await MediaPicker.PickVideoAsync(new MediaPickerOptions
            {
                Title = LocalizedStrings.SelectVideo
            });
            if (file == null)
                return null;
            return new VideoFile
            {
                Name = file.FileName,
                Path = file.FullPath
            };
        }

        public async Task<VideoFile> SelectVideoFromFilesAsync()
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Videos,
                PickerTitle = LocalizedStrings.SelectVideo
            });
            if (file == null)
                return null;
            return new VideoFile
            {
                Name = file.FileName,
                Path = file.FullPath
            };
        }

        public async Task<SubtitlesFile> SelectSubtitlesAsync()
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = LocalizedStrings.SelectSubtitles
            });
            if (file == null)
                return null;
            if (Path.GetExtension(file.FileName).ToLower()
                != Shared.Constants.Strings.SrtFileExtension)
                return null;
            return new SubtitlesFile
            {
                Name = file.FileName,
                Path = file.FullPath
            };
        }
    }
}
