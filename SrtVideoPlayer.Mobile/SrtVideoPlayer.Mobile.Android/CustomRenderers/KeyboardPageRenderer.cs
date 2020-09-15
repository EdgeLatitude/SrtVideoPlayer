using Android.Content;
using Android.Runtime;
using Android.Views;
using SrtVideoPlayer.Mobile.Controls;
using SrtVideoPlayer.Shared.Constants;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(KeyboardPage), typeof(SrtVideoPlayer.Mobile.Droid.CustomRenderers.KeyboardPageRenderer))]
namespace SrtVideoPlayer.Mobile.Droid.CustomRenderers
{
    [Preserve(AllMembers = true)]
    public class KeyboardPageRenderer : PageRenderer
    {
        private KeyboardPage Page => Element as KeyboardPage;

        public KeyboardPageRenderer(Context context) : base(context)
        {
            Focusable = true;
            FocusableInTouchMode = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> args)
        {
            base.OnElementChanged(args);

            if (Visibility == ViewStates.Visible)
                RequestFocus();

            Page.Appearing += (sender, innerArgs) =>
            {
                RequestFocus();
            };
        }

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent keyEvent)
        {
            var handled = false;
            var keyCharacter = Convert.ToChar(keyEvent.UnicodeChar);
            var keyCharacterAsString = keyCharacter.ToString();

            // Add support for special commands
            if (keyEvent.IsCtrlPressed)
                switch (keyCharacterAsString.ToLower())
                {
                    case HardwareInput.CopyCharacter:
                        Page?.OnKeyCommand(KeyCommand.Copy);
                        handled = true;
                        break;
                    case HardwareInput.RootCharacter:
                        Page?.OnKeyCommand(KeyCommand.RootOperator);
                        handled = true;
                        break;
                }
            // Add support for enter and equals key
            else if (keyCode == Keycode.Enter
                || keyCharacterAsString == HardwareInput.ResultOperator)
            {
                Page?.OnKeyCommand(KeyCommand.Calculate);
                handled = true;
            }
            // Add support for backspace key
            else if (keyCode == Keycode.Del)
            {
                Page?.OnKeyCommand(KeyCommand.Delete);
                handled = true;
            }
            else
            {
                // Add support for numbers
                if (char.IsDigit(keyCharacter))
                    handled = true;
                // Add support for parentheses, decimal separators and operators
                else if (HardwareInput.ParenthesesDecimalSeparatorsAndOperators.Contains(keyCharacterAsString))
                    handled = true;

                if (handled)
                    Page?.OnKeyUp(keyCharacter);
            }

            return handled || base.OnKeyUp(keyCode, keyEvent);
        }
    }
}
