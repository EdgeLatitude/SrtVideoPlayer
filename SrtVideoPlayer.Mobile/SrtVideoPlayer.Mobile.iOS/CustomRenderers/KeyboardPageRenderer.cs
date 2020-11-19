using Foundation;
using SrtVideoPlayer.Mobile.Controls;
using SrtVideoPlayer.Shared.Constants;
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
        private const string _spaceKey = "\u0020";

        private readonly IList<UIKeyCommand> _keyCommands = new List<UIKeyCommand>();

        private KeyboardPage Page => Element as KeyboardPage;

        public override bool CanBecomeFirstResponder => true;

        protected override void OnElementChanged(VisualElementChangedEventArgs args)
        {
            base.OnElementChanged(args);

            if (!_keyCommands.Any())
            {
                var selector = new ObjCRuntime.Selector(_keySelector);

                // Add support for space key
                _keyCommands.Add(UIKeyCommand.Create((NSString)_spaceKey, 0, selector));

                // Add support for playback keys
                foreach (var symbol in HardwareInput.KeyboardShortcutsCollection)
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
                if (keyCommand.Input == _spaceKey
                    || keyCommand.Input == KeyboardShortcuts.PlayPauseA)
                    Page?.OnKeyCommand(Controls.KeyCommand.PlayPause);
                else if (char.TryParse(keyCommand.Input, out var keyCharacter))
                {
                    var handled = false;
                    var keyCharacterAsString = keyCharacter.ToString();

                    if (HardwareInput.KeyboardShortcutsCollection.Contains(keyCharacterAsString))
                        handled = true;

                    if (handled)
                        Page?.OnKeyUp(keyCharacter);
                }
            }
        }
    }
}
