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
        private readonly string _videoUri;

        private bool _firstAppearance = true;
        private bool _subtitleCopiedToClipboardToastIsVisible;
        private int _subtitleCopiedToClipboardToastActiveTaps;
        private bool _playbackControlsAreVisible;
        private int _playbackControlsActiveTaps;
        private PlayerViewModel _viewModel;

        public PlayerPage() =>
            SharedInitialization();

        public PlayerPage(string videoUri)
        {
            SharedInitialization();
            _videoUri = videoUri;
        }

        private void SharedInitialization()
        {
            InitializeComponent();
            _viewModel = ViewModelLocator.Instance.Resolve<PlayerViewModel>();
            BindingContext = _viewModel;
            _viewModel.PlayPauseRequested += Player_PlayPauseRequested;
            _viewModel.StopRequested += Player_StopRequested;
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
                case KeyCommand.FullscreenOff:
                    _viewModel.FullscreenOffCommand.Execute(null);
                    break;
            }
        }

        protected override async void OnAppearing()
        {
            if (_firstAppearance
                && !string.IsNullOrWhiteSpace(_videoUri))
                await _viewModel.LoadVideo(_videoUri);
            _firstAppearance = false;
            base.OnAppearing();
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

        private void Player_Tapped(object sender, EventArgs args) =>
            PlaybackControlsAnimation();

        private void PlaybackControls_Tapped(object sender, EventArgs args) =>
            PlaybackControlsAnimation();

        private void PlaybackControlsAnimation()
        {
            if (!_playbackControlsAreVisible)
            {
                PlaybackControls.FadeTo(0.75);
                PlaybackControls.InputTransparent = false;
                _playbackControlsAreVisible = true;
            }

            _playbackControlsActiveTaps++;

            Device.StartTimer(TimeSpan.FromSeconds(3.75), () =>
            {
                _playbackControlsActiveTaps--;

                if (_playbackControlsActiveTaps == 0)
                {
                    PlaybackControls.FadeTo(0);
                    PlaybackControls.InputTransparent = true;
                    _playbackControlsAreVisible = false;
                }

                return false;
            });
        }

        private void Player_PlayPauseRequested(object sender, EventArgs args)
        {
            switch (Player.CurrentState)
            {
                case MediaElementState.Playing:
                    Player.Pause();
                    PlayOrPauseButton.Source = (string)Application.Current.Resources["PlayImage"];
                    break;
                case MediaElementState.Paused:
                    Player.Play();
                    PlayOrPauseButton.Source = (string)Application.Current.Resources["PauseImage"];
                    break;
            }
        }

        private void Player_StopRequested(object sender, EventArgs args) =>
            Player.Stop();

        private void Player_MediaEnded(object sender, EventArgs args)
        {

        }

        private void Player_MediaFailed(object sender, EventArgs args)
        {

        }

        private void Player_MediaOpened(object sender, EventArgs args) =>
            PlaybackControlsAnimation();

        private void Player_SeekCompleted(object sender, EventArgs args)
        {

        }
    }
}
