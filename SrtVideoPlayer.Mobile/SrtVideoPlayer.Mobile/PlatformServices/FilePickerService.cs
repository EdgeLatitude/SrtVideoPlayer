using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Models.Files;
using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class FilePickerService : IFilePickerService
    {
        public async Task<VideoFile> SelectVideoAsync()
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
            return new SubtitlesFile
            {
                Name = file.FileName,
                Path = file.FullPath
            };
        }
    }
}
