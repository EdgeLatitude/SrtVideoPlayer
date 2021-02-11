using MediaManager;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Player;
using SrtVideoPlayer.Mobile.Controls;
using SrtVideoPlayer.Shared.Localization;
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
        private readonly PlayerViewModel _viewModel;

        private bool _firstAppearance = true;
        private bool _subtitleCopiedToClipboardToastIsVisible;
        private int _subtitleCopiedToClipboardToastActiveTaps;
        private bool _playbackControlsAreVisible;
        private int _playbackControlsActiveTaps;
        private int _lastVideoHeight;
        private int _lastVideoWidth;

        // Required for Activator in ThemingService.
        public PlayerPage()
        {
            _viewModel = ViewModelLocator.Instance.Resolve<PlayerViewModel>();
            SharedInitialization();
        }

        public PlayerPage(string videoUri = null)
        {
            _videoUri = videoUri;
            _viewModel = ViewModelLocator.Instance.Resolve<PlayerViewModel>();
            SharedInitialization();
        }

        private void SharedInitialization()
        {
            BindingContext = _viewModel;

            _viewModel.PlayPauseRequested += Player_PlayPauseRequested;
            _viewModel.StopRequested += Player_StopRequested;

            CrossMediaManager.Current.BufferedChanged += Player_BufferedChanged;
            CrossMediaManager.Current.MediaItemFailed += Player_MediaItemFailed;
            CrossMediaManager.Current.MediaItemChanged += Player_MediaItemChanged;
            CrossMediaManager.Current.MediaItemFinished += Player_MediaItemFinished;
            CrossMediaManager.Current.PositionChanged += Player_PositionChanged;

            InitializeComponent();

            SizeChanged += PlayerPage_SizeChanged;
        }

        ~PlayerPage()
        {
            _viewModel.PlayPauseRequested -= Player_PlayPauseRequested;
            _viewModel.StopRequested -= Player_StopRequested;

            CrossMediaManager.Current.BufferedChanged -= Player_BufferedChanged;
            CrossMediaManager.Current.MediaItemFailed -= Player_MediaItemFailed;
            CrossMediaManager.Current.MediaItemChanged -= Player_MediaItemChanged;
            CrossMediaManager.Current.MediaItemFinished -= Player_MediaItemFinished;
            CrossMediaManager.Current.PositionChanged -= Player_PositionChanged;

            SizeChanged -= PlayerPage_SizeChanged;
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

        private void SubtitleLabel_Tapped(object sender, EventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(SubtitleLabel.Text))
                CopySubtitleToClipboardAnimation();
        }

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
            switch (Player.State)
            {
                case MediaPlayerState.Playing:
                    CrossMediaManager.Current.Pause();
                    PlayOrPauseButton.Source = (string)Application.Current.Resources["PlayImage"];
                    break;
                case MediaPlayerState.Paused:
                    CrossMediaManager.Current.Play();
                    PlayOrPauseButton.Source = (string)Application.Current.Resources["PauseImage"];
                    break;
            }
        }

        private void Player_StopRequested(object sender, EventArgs args) =>
            CrossMediaManager.Current.Stop();

        private void Player_BufferedChanged(object sender, BufferedChangedEventArgs args)
        {

        }

        private void Player_MediaItemChanged(object sender, MediaItemEventArgs args)
        {
            _viewModel.MediaLoaded = true;
            PlaybackControlsAnimation();
        }

        private async void Player_MediaItemFailed(object sender, MediaItemFailedEventArgs args) =>
            await DisplayAlert(LocalizedStrings.Error, LocalizedStrings.MediaError, LocalizedStrings.Ok);

        private void Player_MediaItemFinished(object sender, MediaItemEventArgs args) =>
            PlaybackControlsAnimation();

        private void Player_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs args)
        {
            _viewModel.Position = args.Position;
            SetSubtitlesPosition(false);
        }

        private void PlayerPage_SizeChanged(object sender, EventArgs args) =>
            SetSubtitlesPosition(true);

        private void SetSubtitlesPosition(bool pageSizeChanged)
        {
            var videoHeight = Player.VideoHeight;
            var videoWidth = Player.VideoWidth;

            if (videoHeight == default
                || videoWidth == default)
                return;

            if (!pageSizeChanged
                && videoHeight == _lastVideoHeight
                && videoWidth == _lastVideoWidth)
                return;

            _lastVideoHeight = videoHeight;
            _lastVideoWidth = videoWidth;

            var playerHeight = Player.Height;
            var playerWidth = Player.Width;

            var videoAspectRatio = _lastVideoWidth / _lastVideoHeight;
            var playerAspectRatio = playerWidth / playerHeight;

            var scaledVideoHeight = videoAspectRatio > playerAspectRatio ?
                _lastVideoHeight * playerWidth / _lastVideoWidth :
                playerHeight;
            var bottonMargin = (playerHeight / 2) - (scaledVideoHeight / 2) + 8;

            Device.BeginInvokeOnMainThread(() =>
                SubtitleLabel.Margin = new Thickness(16, 8, 16, bottonMargin));
        }
    }
}
