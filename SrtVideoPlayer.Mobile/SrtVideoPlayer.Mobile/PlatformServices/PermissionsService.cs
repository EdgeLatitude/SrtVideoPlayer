using SrtVideoPlayer.Shared.PlatformServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SrtVideoPlayer.Mobile.PlatformServices
{
    class PermissionsService : IPermissionsService
    {
        public async Task<bool> CheckMediaAccessPermission() =>
            await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted;

        public async Task<bool> RequestMediaAccessPermission()
        {
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
