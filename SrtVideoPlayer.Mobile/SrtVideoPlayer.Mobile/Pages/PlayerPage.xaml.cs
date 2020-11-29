using SrtVideoPlayer.Mobile.Controls;
using SrtVideoPlayer.Shared.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SrtVideoPlayer.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerPage : KeyboardPage
    {
        private readonly PlayerViewModel _viewModel;

        private bool _subtitleCopiedToClipboardToastIsVisible;
        private int _subtitleCopiedToClipboardToastActiveTaps;

        public PlayerPage()
        {
            InitializeComponent();
            _viewModel = ViewModelLocator.Instance.Resolve<PlayerViewModel>();
            BindingContext = _viewModel;
        }

        public override void OnKeyUp(string character) =>
            _viewModel.ManageInputFromHardwareCommand.Execute(character);

        public override void OnKeyCommand(KeyCommand command)
        {
            switch (command)
            {
                case KeyCommand.PlayPause:
                    _viewModel.PlayOrPauseCommand.Execute(null);
                    break;
                case KeyCommand.Back5_Seconds:
                    _viewModel.GoBack5_SecondsCommand.Execute(null);
                    break;
                case KeyCommand.Forward5_Seconds:
                    _viewModel.GoForward5_SecondsCommand.Execute(null);
                    break;
                case KeyCommand.FullScreenOff:
                    _viewModel.ExitFullScreenCommand.Execute(null);
                    break;
            }
        }

        private void SubtitleLabel_Tapped(object sender, EventArgs args) =>
            CopySubtitleToClipboardAnimation();

        private void CopySubtitleToClipboardAnimation()
        {
            if (!_subtitleCopiedToClipboardToastIsVisible)
            {
                SubtitleCopiedToClipboardToast.FadeTo(0.75);
                _subtitleCopiedToClipboardToastIsVisible = true;
            }

            _subtitleCopiedToClipboardToastActiveTaps++;

            Device.StartTimer(TimeSpan.FromSeconds(3.75), () =>
            {
                _subtitleCopiedToClipboardToastActiveTaps--;

                if (_subtitleCopiedToClipboardToastActiveTaps == 0)
                {
                    SubtitleCopiedToClipboardToast.FadeTo(0);
                    _subtitleCopiedToClipboardToastIsVisible = false;
                }

                return false;
            });
        }

        private void MediaElement_MediaEnded(object sender, EventArgs args)
        {

        }

        private void MediaElement_MediaFailed(object sender, EventArgs args)
        {

        }

        private void MediaElement_MediaOpened(object sender, EventArgs args)
        {

        }

        private void MediaElement_SeekCompleted(object sender, EventArgs args)
        {

        }
    }
}
