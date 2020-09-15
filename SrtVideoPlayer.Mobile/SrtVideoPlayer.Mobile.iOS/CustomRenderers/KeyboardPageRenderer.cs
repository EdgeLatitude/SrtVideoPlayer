using SrtVideoPlayer.Mobile.Controls;
using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Localization;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(KeyboardPage), typeof(SrtVideoPlayer.Mobile.iOS.CustomRenderers.KeyboardPageRenderer))]
namespace SrtVideoPlayer.Mobile.iOS.CustomRenderers
{
    public class KeyboardPageRenderer : PageRenderer
    {
        private const string _keySelector = "KeyCommand:";
        private const string _enterKey = "\r";
        private const string _backspaceKey = "\u0008";

        private readonly IList<UIKeyCommand> _keyCommands = new List<UIKeyCommand>();

        private KeyboardPage Page => Element as KeyboardPage;

        public override bool CanBecomeFirstResponder => true;

        protected override void OnElementChanged(VisualElementChangedEventArgs args)
        {
            base.OnElementChanged(args);

            if (!_keyCommands.Any())
            {
                var selector = new ObjCRuntime.Selector(_keySelector);

                // Add support for special commands (viewable on iPad (>= iOS 9) when holding down ⌘)
                _keyCommands.Add(UIKeyCommand.Create(new NSString(HardwareInput.CopyCharacter), UIKeyModifierFlags.Command, selector, new NSString(LocalizedStrings.Copy)));
                _keyCommands.Add(UIKeyCommand.Create(new NSString(HardwareInput.RootCharacter), UIKeyModifierFlags.Command, selector, new NSString(LocalizedStrings.RootOperator)));

                // Add support for enter and equals key
                _keyCommands.Add(UIKeyCommand.Create((NSString)_enterKey, 0, selector));
                _keyCommands.Add(UIKeyCommand.Create((NSString)HardwareInput.ResultOperator, 0, selector));

                // Add support for backspace key
                _keyCommands.Add(UIKeyCommand.Create((NSString)_backspaceKey, 0, selector));

                // Add support for numbers
                for (var i = 0; i < 10; i++)
                    _keyCommands.Add(UIKeyCommand.Create((NSString)i.ToString(), 0, selector));

                // Add support for parentheses, decimal separators and operators
                foreach (var symbol in HardwareInput.ParenthesesDecimalSeparatorsAndOperators)
                    _keyCommands.Add(UIKeyCommand.Create((NSString)symbol, 0, selector));

                foreach (var kc in _keyCommands)
                    AddKeyCommand(kc);
            }
        }

        [Export(_keySelector)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Remove unused member", Justification = "It is used but not detected by the IDE")]
        private void KeyCommand(UIKeyCommand keyCommand)
        {
            if (keyCommand == null)
                return;

            if (_keyCommands.Contains(keyCommand))
            {
                if (keyCommand.ModifierFlags == UIKeyModifierFlags.Command)
                    switch (keyCommand.Input.ToString())
                    {
                        case HardwareInput.CopyCharacter:
                            Page?.OnKeyCommand(Controls.KeyCommand.Copy);
                            break;
                        case HardwareInput.RootCharacter:
                            Page?.OnKeyCommand(Controls.KeyCommand.RootOperator);
                            break;
                    }
                else if (keyCommand.Input == _enterKey
                    || keyCommand.Input == HardwareInput.ResultOperator)
                    Page?.OnKeyCommand(Controls.KeyCommand.Calculate);
                else if (keyCommand.Input == _backspaceKey)
                    Page?.OnKeyCommand(Controls.KeyCommand.Delete);
                else if (char.TryParse(keyCommand.Input, out var keyCharacter))
                {
                    var handled = false;
                    var keyCharacterAsString = keyCharacter.ToString();

                    if (char.IsDigit(keyCharacter))
                        handled = true;
                    else if (HardwareInput.ParenthesesDecimalSeparatorsAndOperators.Contains(keyCharacterAsString))
                        handled = true;

                    if (handled)
                        Page?.OnKeyUp(keyCharacter);
                }
            }
        }
    }
}
