using Android.Content;
using Android.Widget;
using My9GAG.Views.CustomViews.VideoPlayer;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ARelativeLayout = Android.Widget.RelativeLayout;

[assembly: ExportRenderer(typeof(VideoPlayer), typeof(My9GAG.Droid.Renderers.VideoPlayerRenderer))]

namespace My9GAG.Droid.Renderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, ARelativeLayout>
    {
        #region Constructors

        public VideoPlayerRenderer(Context context) : base(context)
        {

        }

        #endregion

        #region Handlers

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    _videoView = new VideoView(Context);

                    ARelativeLayout relativeLayout = new ARelativeLayout(Context);
                    relativeLayout.AddView(_videoView);

                    ARelativeLayout.LayoutParams layoutParams =
                        new ARelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    layoutParams.AddRule(LayoutRules.CenterInParent);
                    _videoView.LayoutParameters = layoutParams;

                    _videoView.Prepared += OnVideoViewPrepared;

                    SetNativeControl(relativeLayout);
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
                if (Math.Abs(_videoView.CurrentPosition - Element.Position.TotalMilliseconds) > 1000)
                {
                    _videoView.SeekTo((int)Element.Position.TotalMilliseconds);
                }
            }
        }

        void OnVideoViewPrepared(object sender, EventArgs args)
        {
            _isPrepared = true;
            ((IVideoPlayer)Element).Duration = TimeSpan.FromMilliseconds(_videoView.Duration);
        }

        void OnUpdateStatus(object sender, EventArgs args)
        {
            VideoPlayerStatus status = VideoPlayerStatus.Loading;

            if (_isPrepared)
            {
                status = _videoView.IsPlaying ? VideoPlayerStatus.Playing : VideoPlayerStatus.Paused;
            }

            ((IVideoPlayer)Element).Status = status;

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(_videoView.CurrentPosition);
            ((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, timeSpan);
        }

        void OnPlayRequested(object sender, EventArgs args)
        {
            _videoView.Start();
        }

        void OnPauseRequested(object sender, EventArgs args)
        {
            _videoView.Pause();
        }

        void OnStopRequested(object sender, EventArgs args)
        {
            _videoView.StopPlayback();
        }

        #endregion

        #region Implementation

        protected override void Dispose(bool disposing)
        {
            if (Control != null && _videoView != null)
            {
                _videoView.Prepared -= OnVideoViewPrepared;
            }
            if (Element != null)
            {
                Element.UpdateStatus -= OnUpdateStatus;
            }

            base.Dispose(disposing);
        }

        private void SetAreTransportControlsEnabled()
        {
            if (Element.AreTransportControlsEnabled)
            {
                _mediaController = new MediaController(Context);
                _mediaController.SetMediaPlayer(_videoView);
                _videoView.SetMediaController(_mediaController);
            }
            else
            {
                _videoView.SetMediaController(null);

                if (_mediaController != null)
                {
                    _mediaController.SetMediaPlayer(null);
                    _mediaController = null;
                }
            }
        }

        private void SetSource()
        {
            _isPrepared = false;
            bool hasSetSource = false;

            if (Element.Source is UriVideoSource)
            {
                string uri = (Element.Source as UriVideoSource).Uri;

                if (!string.IsNullOrWhiteSpace(uri))
                {
                    _videoView.SetVideoURI(Android.Net.Uri.Parse(uri));
                    hasSetSource = true;
                }
            }

            if (hasSetSource && Element.AutoPlay)
            {
                _videoView.Start();
            }
        }

        #endregion

        #region Fields

        private VideoView _videoView;
        private MediaController _mediaController;
        private bool _isPrepared;

        #endregion
    }
}
