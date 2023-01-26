﻿using MediaManager;
using MediaManager.Forms;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Player;
using MediaManager.Video;
using SrtVideoPlayer.Mobile.Controls;
using SrtVideoPlayer.Shared.Extensions;
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

        public VideoFile VideoFile { get; set; }

        public PlayerPage()
        {
            _viewModel = ViewModelLocator.Instance.ResolveViewModel<PlayerViewModel>();
            BindingContext = _viewModel;

            _viewModel.PlayPauseRequested += ViewModel_PlayPauseRequested;
            _viewModel.SeekRequested += ViewModel_SeekRequested;
            _viewModel.StopRequested += ViewModel_StopRequested;

            CrossMediaManager.Current.MediaItemFailed += Player_MediaItemFailed;
            CrossMediaManager.Current.MediaItemFinished += Player_MediaItemFinished;
            CrossMediaManager.Current.PositionChanged += Player_PositionChanged;
            CrossMediaManager.Current.StateChanged += Player_StateChanged;

            InitializeComponent();

            ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;
        }

        ~PlayerPage()
        {
            _viewModel.PlayPauseRequested -= ViewModel_PlayPauseRequested;
            _viewModel.SeekRequested -= ViewModel_SeekRequested;
            _viewModel.StopRequested -= ViewModel_StopRequested;

            CrossMediaManager.Current.MediaItemFailed -= Player_MediaItemFailed;
            CrossMediaManager.Current.MediaItemFinished -= Player_MediaItemFinished;
            CrossMediaManager.Current.PositionChanged -= Player_PositionChanged;
            CrossMediaManager.Current.StateChanged -= Player_StateChanged;

            ProgressSlider.ValueChanged -= ProgressSlider_ValueChanged;
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
                _viewModel.LaunchAsync(VideoFile).AwaitInOtherContext(true);
                _firstAppearance = false;
            }
            // iOS bug fix.
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

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (_viewModel.MediaLoaded)
                SetSubtitlesPosition(true);
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
            {
                if (!_viewModel.SubtitlesLocationSet)
                    Device.BeginInvokeOnMainThread(() => _viewModel.SubtitlesLocationSet = true);
                return;
            }

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
            {
                SubtitleLabel.Margin = new Thickness(horizontalMargin, verticalMargin, horizontalMargin, bottonMargin);
                _viewModel.SubtitlesLocationSet = true;
            });
        }

        #region ViewModel events
        private async void ViewModel_PlayPauseRequested(object sender, EventArgs args)
        {
            switch (CrossMediaManager.Current.State)
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
        #endregion

        #region Player events
        private async void Player_MediaItemFailed(object sender, MediaItemFailedEventArgs args)
        {
            if (Player.State != MediaPlayerState.Failed)
                return;
            _viewModel.SelectingVideo = false;
            await DisplayAlert(LocalizedStrings.Error, $"{LocalizedStrings.MediaError}{Environment.NewLine}{args.Message}", LocalizedStrings.Ok);
            _viewModel.StopCommand.Execute(null);
        }

        private void Player_MediaItemFinished(object sender, MediaItemEventArgs args) =>
            _viewModel.StopCommand.Execute(null);

        private void Player_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(_viewModel.Source?.Location))
                return;

            if (!_viewModel.MediaLoaded)
                Device.BeginInvokeOnMainThread(() =>
                {
                    _viewModel.MediaLoaded = true;
                    _viewModel.SelectingVideo = false;
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
            {
                Device.BeginInvokeOnMainThread(() =>
                    ProgressSlider.Value = (double)_viewModel.LastPositionFromHistory.Value.Ticks / duration.Ticks);
                return;
            }

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
        #endregion

        #region Other events
        private void PlaybackControls_Tapped(object sender, EventArgs args) =>
            PlaybackControlsAnimation();

        private void PlayerPinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs args)
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

        private void PlayerTapGestureRecognizer_Tapped(object sender, EventArgs args)
        {
            if (_playbackControlsAreVisible)
                _viewModel.PlayOrPauseCommand.Execute(null);
            PlaybackControlsAnimation();
        }

        private async void ProgressSlider_ValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (_progressSliderIsLocked)
                return;
            var duration = CrossMediaManager.Current.Duration;
            var position = duration * args.NewValue;
            await CrossMediaManager.Current.SeekTo(position);
        }

        private void SubtitleLabel_Tapped(object sender, EventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(SubtitleLabel.Text))
                CopySubtitleToClipboardAnimation();
        }
        #endregion
    }
}
