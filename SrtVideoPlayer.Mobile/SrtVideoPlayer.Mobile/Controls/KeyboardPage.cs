using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.Controls
{
    public abstract class KeyboardPage : ContentPage
    {
        public abstract void OnKeyUp(string character);
        public abstract void OnKeyCommand(KeyCommand command);
    }

    public enum KeyCommand
    {
        PlayPause,
        Back5_Seconds,
        Forward5_Seconds,
        FullscreenOff
    }
}
