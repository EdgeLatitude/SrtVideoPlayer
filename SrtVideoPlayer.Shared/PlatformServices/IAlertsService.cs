using SrtVideoPlayer.Shared.Models.Enums;
using System.Threading.Tasks;

namespace SrtVideoPlayer.Shared.PlatformServices
{
    public interface IAlertsService
    {
        Task DisplayAlertAsync(string title, string message);
        Task<bool> DisplayConfirmationAsync(string title, string message, string action);
        Task<string> DisplayOptionsAsync(string title, string destruction, params string[] options);
        Task<string> DisplayPromptAsync(string title, string message, string placeholder = null,
            int? maxLength = null, string initialValue = null, KeyboardType keyboardType = KeyboardType.Plain);
    }
}
