using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class PermissionsService : IPermissionsService
    {
        public async Task<bool> CheckMediaAccessPermission() =>
            await Permissions.CheckStatusAsync<Permissions.Media>() == PermissionStatus.Granted
                && await Permissions.CheckStatusAsync<Permissions.Photos>() == PermissionStatus.Granted
                && await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted;

        public async Task<bool> RequestMediaAccessPermission()
        {
            var mediaAccessStatus = await Permissions.CheckStatusAsync<Permissions.Media>();
            switch (mediaAccessStatus)
            {
                case PermissionStatus.Unknown:
                    if (await Permissions.RequestAsync<Permissions.Media>() != PermissionStatus.Granted)
                        return false;
                    break;
                case PermissionStatus.Denied:
                case PermissionStatus.Disabled:
                case PermissionStatus.Restricted:
                    return false;
            }

            var photosAccessStatus = await Permissions.CheckStatusAsync<Permissions.Photos>();
            switch (photosAccessStatus)
            {
                case PermissionStatus.Unknown:
                    if (await Permissions.RequestAsync<Permissions.Media>() != PermissionStatus.Granted)
                        return false;
                    break;
                case PermissionStatus.Denied:
                case PermissionStatus.Disabled:
                case PermissionStatus.Restricted:
                    return false;
            }

            var storageReadAccessStatus = await Permissions.CheckStatusAsync<Permissions.Photos>();
            switch (storageReadAccessStatus)
            {
                case PermissionStatus.Unknown:
                    if (await Permissions.RequestAsync<Permissions.Media>() != PermissionStatus.Granted)
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
