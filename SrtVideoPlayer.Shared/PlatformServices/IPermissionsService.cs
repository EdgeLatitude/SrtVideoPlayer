using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IPermissionsService
    {
        Task<bool> CheckMediaAccessPermission();
        Task<bool> RequestMediaAccessPermission();
    }
}
