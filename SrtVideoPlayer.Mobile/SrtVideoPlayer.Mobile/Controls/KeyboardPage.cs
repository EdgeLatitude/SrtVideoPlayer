using Xamarin.Forms;

namespace SrtVideoPlayer.Mobile.Controls
{
    public class KeyboardPage : ContentPage
    {
        public virtual void OnKeyUp(char character) { return; }
        public virtual void OnKeyCommand(KeyCommand command) { return; }
    }

    public enum KeyCommand
    {
        Copy,
        RootOperator,
        Calculate,
        Delete
    }
}
