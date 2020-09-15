using SrtVideoPlayer.Mobile.Controls;
using SrtVideoPlayer.Shared.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SrtVideoPlayer.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerPage : KeyboardPage
    {
        private readonly PlayerViewModel _viewModel;

        private bool _inputCopiedToClipboardToastIsVisible;
        private int _inputCopiedToClipboardToastActiveTaps;

        public PlayerPage()
        {
            InitializeComponent();
            _viewModel = ViewModelLocator.Instance.Resolve<PlayerViewModel>();
            BindingContext = _viewModel;
        }

        public override void OnKeyUp(char character) =>
            _viewModel.ManageInputFromHardwareCommand.Execute(character);

        public override void OnKeyCommand(KeyCommand command)
        {
            switch (command)
            {
                case KeyCommand.Copy:
                    _viewModel.CopyInputToClipboardCommand.Execute(null);
                    CopyInputToClipboardAnimation();
                    break;
                case KeyCommand.RootOperator:
                    SquareRootButton.Command.Execute(SquareRootButton.CommandParameter);
                    break;
                case KeyCommand.Calculate:
                    CalculateButton.Command.Execute(null);
                    break;
                case KeyCommand.Delete:
                    DeleteButton.Command.Execute(null);
                    break;
            }
        }

        private async void InputLabel_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(Width))
                return;

            await InputScrollView.ScrollToAsync(InputLabel, _viewModel.AfterResult ? ScrollToPosition.Start : ScrollToPosition.End, false);
        }

        private void InputLabel_Tapped(object sender, EventArgs args) =>
            CopyInputToClipboardAnimation();

        private void CopyInputToClipboardAnimation()
        {
            if (!_inputCopiedToClipboardToastIsVisible)
            {
                InputCopiedToClipboardToast.FadeTo(0.75);
                _inputCopiedToClipboardToastIsVisible = true;
            }

            _inputCopiedToClipboardToastActiveTaps++;

            Device.StartTimer(TimeSpan.FromSeconds(3.75), () =>
            {
                _inputCopiedToClipboardToastActiveTaps--;

                if (_inputCopiedToClipboardToastActiveTaps == 0)
                {
                    InputCopiedToClipboardToast.FadeTo(0);
                    _inputCopiedToClipboardToastIsVisible = false;
                }

                return false;
            });
        }
    }
}
