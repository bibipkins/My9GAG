using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AVFoundation;
using AVKit;
using CoreMedia;
using Foundation;
using My9GAG.Views.CustomViews.VideoPlayer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoPlayer), typeof(My9GAG.iOS.Renderers.VideoPlayerRenderer))]

namespace My9GAG.iOS.Renderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, UIView>
    {
        #region Properties

        public override UIViewController ViewController => _playerViewController;

        #endregion

        #region Handlers

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    _playerViewController = new AVPlayerViewController();
                    _player = new AVPlayer();
                    _playerViewController.Player = _player;

                    SetNativeControl(_playerViewController.View);
                }

                SetAreTransportControlsEnabled();
                SetSource();

                args.NewElement.UpdateStatus += OnUpdateStatus;
                args.NewElement.PlayRequested += OnPlayRequested;
                args.NewElement.PauseRequested += OnPauseRequested;
                args.NewElement.StopRequested += OnStopRequested;
            }

            if (args.OldElement != null)
            {
                args.OldElement.UpdateStatus -= OnUpdateStatus;
                args.OldElement.PlayRequested -= OnPlayRequested;
                args.OldElement.PauseRequested -= OnPauseRequested;
                args.OldElement.StopRequested -= OnStopRequested;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == VideoPlayer.AreTransportControlsEnabledProperty.PropertyName)
            {
                SetAreTransportControlsEnabled();
            }
            else if (args.PropertyName == VideoPlayer.SourceProperty.PropertyName)
            {
                SetSource();
            }
            else if (args.PropertyName == VideoPlayer.PositionProperty.PropertyName)
            {
                TimeSpan controlPosition = ConvertTime(_player.CurrentTime);

                if (Math.Abs((controlPosition - Element.Position).TotalSeconds) > 1)
                {
                    _player.Seek(CMTime.FromSeconds(Element.Position.TotalSeconds, 1));
                }
            }
        }

        private void OnUpdateStatus(object sender, EventArgs args)
        {
            VideoPlayerStatus videoStatus = VideoPlayerStatus.Loading;

            switch (_player.Status)
            {
                case AVPlayerStatus.ReadyToPlay:
                    switch (_player.TimeControlStatus)
                    {
                        case AVPlayerTimeControlStatus.Playing:
                            videoStatus = VideoPlayerStatus.Playing;
                            break;

                        case AVPlayerTimeControlStatus.Paused:
                            videoStatus = VideoPlayerStatus.Paused;
                            break;
                    }
                    break;
            }

            ((IVideoPlayer)Element).Status = videoStatus;

            if (_playerItem != null)
            {
                ((IVideoPlayer)Element).Duration = ConvertTime(_playerItem.Duration);
                ((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, ConvertTime(_playerItem.CurrentTime));
            }
        }

        private void OnPlayRequested(object sender, EventArgs args)
        {
            _player.Play();
        }

        private void OnPauseRequested(object sender, EventArgs args)
        {
            _player.Pause();
        }

        private void OnStopRequested(object sender, EventArgs args)
        {
            _player.Pause();
            _player.Seek(new CMTime(0, 1));
        }

        #endregion

        #region Implementation

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_player != null)
            {
                _player.ReplaceCurrentItemWithPlayerItem(null);
            }
        }

        private void SetAreTransportControlsEnabled()
        {
            ((AVPlayerViewController)ViewController).ShowsPlaybackControls = Element.AreTransportControlsEnabled;
        }

        private void SetSource()
        {
            AVAsset asset = null;

            if (Element.Source is UriVideoSource)
            {
                string uri = (Element.Source as UriVideoSource).Uri;

                if (!string.IsNullOrWhiteSpace(uri))
                {
                    asset = AVAsset.FromUrl(new NSUrl(uri));
                }
            }

            if (asset != null)
            {
                _playerItem = new AVPlayerItem(asset);
            }
            else
            {
                _playerItem = null;
            }

            _player.ReplaceCurrentItemWithPlayerItem(_playerItem);

            if (_playerItem != null && Element.AutoPlay)
            {
                _player.Play();
            }
        }

        private TimeSpan ConvertTime(CMTime cmTime)
        {
            return TimeSpan.FromSeconds(double.IsNaN(cmTime.Seconds) ? 0 : cmTime.Seconds);
        }

        #endregion

        #region Fields

        private AVPlayer _player;
        private AVPlayerItem _playerItem;
        private AVPlayerViewController _playerViewController;

        #endregion
    }
}
