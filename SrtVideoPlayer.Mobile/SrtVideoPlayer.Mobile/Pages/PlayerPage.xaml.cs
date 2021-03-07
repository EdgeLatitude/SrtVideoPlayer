using MediaManager;
using MediaManager.Forms;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Player;
using MediaManager.Video;
using SrtVideoPlayer.Mobile.Controls;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Models.Files;
using SrtVideoPlayer.Shared.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SrtVideoPlayer.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerPage : KeyboardPage
    {
        private const double _defaultPinchScale = 1d;

        private readonly VideoFile _videoFile;
        private readonly PlayerViewModel _viewModel;

        private IVideoView _videoView;
        private double _pinchScale;
        private bool _firstAppearance = true;
        private bool _subtitleCopiedToClipboardToastIsVisible;
        private int _subtitleCopiedToClipboardToastActiveTaps;
        private bool _playbackControlsAreVisible;
        private int _playbackControlsActiveTaps;
        private int _lastVideoHeight;
        private int _lastVideoWidth;
        private bool _progressSliderIsLocked;
        private bool _firstPlayback = true;

        // Required for Activator in ThemingService.
        public PlayerPage()
        {
            _viewModel = ViewModelLocator.Instance.Resolve<PlayerViewModel>();
            SharedInitialization();
        }

        public PlayerPage(VideoFile videoFile = null)
        {
            _videoFile = videoFile;
            _viewModel = ViewModelLocator.Instance.Resolve<PlayerViewModel>();
            SharedInitialization();
        }

        private void SharedInitialization()
        {
            BindingContext = _viewModel;

            _viewModel.PlayPauseRequested += ViewModel_PlayPauseRequested;
            _viewModel.SeekRequested += ViewModel_SeekRequested;
            _viewModel.StopRequested += ViewModel_StopRequested;

            CrossMediaManager.Current.BufferedChanged += Player_BufferedChanged;
            CrossMediaManager.Current.MediaItemFailed += Player_MediaItemFailed;
            CrossMediaManager.Current.MediaItemFinished += Player_MediaItemFinished;
            CrossMediaManager.Current.PositionChanged += Player_PositionChanged;
            CrossMediaManager.Current.StateChanged += Player_StateChanged;

            InitializeComponent();
        }

        ~PlayerPage()
        {
            _viewModel.PlayPauseRequested -= ViewModel_PlayPauseRequested;
            _viewModel.SeekRequested -= ViewModel_SeekRequested;
            _viewModel.StopRequested -= ViewModel_StopRequested;

            CrossMediaManager.Current.BufferedChanged -= Player_BufferedChanged;
            CrossMediaManager.Current.MediaItemFailed -= Player_MediaItemFailed;
            CrossMediaManager.Current.MediaItemFinished -= Player_MediaItemFinished;
            CrossMediaManager.Current.PositionChanged -= Player_PositionChanged;
            CrossMediaManager.Current.StateChanged -= Player_StateChanged;
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

        protected override void OnAppearing()
        {
            if (_firstAppearance)
            {
                _ = _viewModel.Launch(_videoFile);
                _firstAppearance = false;
            }
            if (Device.RuntimePlatform == Device.iOS)
                if (CrossMediaManager.Current.MediaPlayer.VideoView == null)
                    CrossMediaManager.Current.MediaPlayer.VideoView = _videoView;
                else
                    _videoView = CrossMediaManager.Current.MediaPlayer.VideoView;
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            if (_viewModel.Fullscreen)
            {
                _viewModel.FullscreenOffCommand.Execute(null);
                return true;
            }
            return false;
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

        private void Player_PinchUpdated(object sender, PinchGestureUpdatedEventArgs args)
        {
            switch (args.Status)
            {
                case GestureStatus.Started:
                    _pinchScale = args.Scale;
                    break;
                case GestureStatus.Running:
                    _pinchScale = args.Scale;
                    break;
                case GestureStatus.Completed:
                    if (_pinchScale > _defaultPinchScale)
                        CrossMediaManager.Current.MediaPlayer.VideoAspect = VideoAspectMode.AspectFill;
                    else if (_pinchScale < _defaultPinchScale)
                        CrossMediaManager.Current.MediaPlayer.VideoAspect = VideoAspectMode.AspectFit;
                    break;
            }
        }

        private void Player_Tapped(object sender, EventArgs args)
        {
            if (_playbackControlsAreVisible)
                _viewModel.PlayOrPauseCommand.Execute(null);
            PlaybackControlsAnimation();
        }

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

        private async void ViewModel_PlayPauseRequested(object sender, EventArgs args)
        {
            switch (Player.State)
            {
                case MediaPlayerState.Playing:
                    await CrossMediaManager.Current.Pause();
                    break;
                case MediaPlayerState.Paused:
                    await CrossMediaManager.Current.Play();
                    break;
            }
        }

        private void ViewModel_SeekRequested(object sender, EventArgs args)
        {
            var duration = CrossMediaManager.Current.Duration;
            _progressSliderIsLocked = false;
            ProgressSlider.Value = (double)_viewModel.Position.Ticks / duration.Ticks;
        }

        private async void ViewModel_StopRequested(object sender, EventArgs args) =>
            await CrossMediaManager.Current.Stop();

        private void Player_BufferedChanged(object sender, BufferedChangedEventArgs args)
        {

        }

        private async void Player_MediaItemFailed(object sender, MediaItemFailedEventArgs args)
        {
            if (Player.State != MediaPlayerState.Failed)
                return;
            await DisplayAlert(LocalizedStrings.Error, $"{LocalizedStrings.MediaError}{Environment.NewLine}{args.Message}", LocalizedStrings.Ok);
            _viewModel.StopCommand.Execute(null);
        }

        private void Player_MediaItemFinished(object sender, MediaItemEventArgs args) =>
            PlaybackControlsAnimation();

        private void Player_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.Source?.Location))
                return;

            if (!_viewModel.MediaLoaded)
                Device.BeginInvokeOnMainThread(() =>
                {
                    _viewModel.MediaLoaded = true;
                    PlaybackControlsAnimation();
                });

            var firstPlayback = _firstPlayback;

            if (firstPlayback)
            {
                _firstPlayback = false;
                _viewModel.Volume = Player.Volume;
                Player.SetBinding(VideoView.VolumeProperty, nameof(_viewModel.Volume));
            }

            var duration = CrossMediaManager.Current.Duration;
            var position = args.Position;
            _viewModel.Duration = duration;
            _viewModel.Position = position;

            if (firstPlayback
                && _viewModel.LastPositionFromHistory.HasValue)
                Device.BeginInvokeOnMainThread(() =>
                    ProgressSlider.Value = (double)_viewModel.LastPositionFromHistory.Value.Ticks / duration.Ticks);
            else
                Device.BeginInvokeOnMainThread(() =>
                {
                    _progressSliderIsLocked = true;
                    ProgressSlider.Value = (double)position.Ticks / duration.Ticks;
                    _progressSliderIsLocked = false;
                });

            SetSubtitlesPosition(false);
        }

        private void Player_StateChanged(object sender, StateChangedEventArgs args)
        {
            _viewModel.Buffering = _viewModel.MediaLoaded
                && args.State == MediaPlayerState.Buffering;
            switch (args.State)
            {
                case MediaPlayerState.Playing:
                    PlayOrPauseButton.Source = (string)Application.Current.Resources["PauseImage"];
                    break;
                case MediaPlayerState.Paused:
                    PlayOrPauseButton.Source = (string)Application.Current.Resources["PlayImage"];
                    break;
            }
        }

        private async void ProgressSlider_ValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (_progressSliderIsLocked)
                return;
            var duration = CrossMediaManager.Current.Duration;
            var position = duration * args.NewValue;
            await CrossMediaManager.Current.SeekTo(position);
        }

        private void Page_SizeChanged(object sender, EventArgs args) =>
            SetSubtitlesPosition(true);

        private void SetSubtitlesPosition(bool pageSizeChanged)
        {
            const double horizontalMargin = 16;
            const double verticalMargin = 8;

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

            _viewModel.LandscapeVideo = _lastVideoWidth > _lastVideoHeight;

            var containerHeight = ContainerGrid.Height;
            var containerWidth = ContainerGrid.Width;

            var videoAspectRatio = (double)_lastVideoWidth / _lastVideoHeight;
            var containerAspectRatio = containerWidth / containerHeight;

            var scaledVideoHeight = CrossMediaManager.Current.MediaPlayer.VideoAspect == VideoAspectMode.AspectFit
                && videoAspectRatio > containerAspectRatio ?
                _lastVideoHeight * containerWidth / _lastVideoWidth :
                containerHeight;
            var bottonMargin = (containerHeight / 2) - (scaledVideoHeight / 2) + verticalMargin;

            Device.BeginInvokeOnMainThread(() =>
                SubtitleLabel.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, bottonMargin));
        }
    }
}
