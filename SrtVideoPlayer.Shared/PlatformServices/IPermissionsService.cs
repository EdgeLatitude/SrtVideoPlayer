using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IPermissionsService
    {
        Task<bool> CheckMediaAndFilesAccessPermissions();
        Task<bool> RequestMediaAndFilesAccessPermissions();
    }
}
