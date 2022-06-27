using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    internal class PermissionsService : IPermissionsService
    {
        public async Task<bool> CheckMediaAndFilesAccessPermissionsAsync() =>
            await Permissions.CheckStatusAsync<Permissions.Photos>() == PermissionStatus.Granted
                && await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted;

        public async Task<bool> RequestMediaAndFilesAccessPermissionsAsync()
        {
            var photosAccessStatus = await Permissions.CheckStatusAsync<Permissions.Photos>();
            switch (photosAccessStatus)
            {
                case PermissionStatus.Unknown:
                    if (await Permissions.RequestAsync<Permissions.Photos>() != PermissionStatus.Granted)
                        return false;
                    break;
                case PermissionStatus.Denied:
                case PermissionStatus.Disabled:
                case PermissionStatus.Restricted:
                    return false;
            }

            var storageReadAccessStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            switch (storageReadAccessStatus)
            {
                case PermissionStatus.Unknown:
                    if (await Permissions.RequestAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
                        return false;
                    break;
                case PermissionStatus.Denied:
                case PermissionStatus.Disabled:
                case PermissionStatus.Restricted:
                    return false;
            }

            return true;
        }
    }
}
