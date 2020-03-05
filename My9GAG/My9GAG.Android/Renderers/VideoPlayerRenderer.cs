using Android.Content;
using Android.Media;
using Android.Widget;
using My9GAG.Views.CustomViews.VideoPlayer;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Media.MediaPlayer;
using ARelativeLayout = Android.Widget.RelativeLayout;

[assembly: ExportRenderer(typeof(VideoPlayerControl), typeof(My9GAG.Droid.Renderers.VideoPlayerRenderer))]

namespace My9GAG.Droid.Renderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayerControl, ARelativeLayout>, IOnPreparedListener
    {
        #region Constructors

        public VideoPlayerRenderer(Context context) : base(context)
        {

        }

        #endregion

        #region Handlers

        public void OnPrepared(MediaPlayer mediaPlayer)
        {
            _mediaPlayer = mediaPlayer;
            _isPrepared = true;

            ((IVideoPlayer)Element).Duration = TimeSpan.FromMilliseconds(_videoView.Duration);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayerControl> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    _videoView = new VideoView(Context);
                    _videoView.SetOnPreparedListener(this);

                    var relativeLayout = new ARelativeLayout(Context);
                    relativeLayout.AddView(_videoView);

                    var layoutParams = new ARelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    layoutParams.AddRule(LayoutRules.CenterInParent);
                    _videoView.LayoutParameters = layoutParams;

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

            if (args.PropertyName == VideoPlayerControl.AreTransportControlsEnabledProperty.PropertyName)
            {
                SetAreTransportControlsEnabled();
            }
            else if (args.PropertyName == VideoPlayerControl.SourceProperty.PropertyName)
            {
                SetSource();
            }
            else if (args.PropertyName == VideoPlayerControl.PositionProperty.PropertyName)
            {
                if (Math.Abs(_videoView.CurrentPosition - Element.Position.TotalMilliseconds) > 1000)
                {
                    _videoView.SeekTo((int)Element.Position.TotalMilliseconds);
                }
            }
            else if (args.PropertyName == VideoPlayerControl.IsMutedProperty.PropertyName)
            {
                if (Element.IsMuted)
                {
                    SetVolume(0);
                }
                else
                {
                    SetVolume(100);
                }
            }
        }

        private void OnUpdateStatus(object sender, EventArgs args)
        {
            VideoPlayerStatus status = VideoPlayerStatus.Loading;

            if (_isPrepared)
            {
                status = _videoView.IsPlaying ? VideoPlayerStatus.Playing : VideoPlayerStatus.Paused;
            }

            ((IVideoPlayer)Element).Status = status;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(_videoView.CurrentPosition);
            ((IElementController)Element).SetValueFromRenderer(VideoPlayerControl.PositionProperty, timeSpan);
        }
        private void OnPlayRequested(object sender, EventArgs args)
        {
            _videoView.Start();
        }
        private void OnPauseRequested(object sender, EventArgs args)
        {
            _videoView.Pause();
        }
        private void OnStopRequested(object sender, EventArgs args)
        {
            _videoView.StopPlayback();
        }

        #endregion

        #region Implementation

        protected override void Dispose(bool disposing)
        {
            if (Element != null)
            {
                Element.UpdateStatus -= OnUpdateStatus;
            }

            base.Dispose(disposing);
        }

        private void SetVolume(int amount)
        {
            int max = 100;
            double numerator = max - amount > 0 ? Math.Log(max - amount) : 0;
            float volume = (float)(1 - (numerator / Math.Log(max)));

            _mediaPlayer.SetVolume(volume, volume);
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

            if (Element.Source is UriVideoSource)
            {
                string uri = (Element.Source as UriVideoSource).Uri;

                if (!string.IsNullOrWhiteSpace(uri))
                {
                    _videoView.SetVideoURI(Android.Net.Uri.Parse(uri));

                    if (Element.AutoPlay)
                    {
                        _videoView.Start();
                    }
                }
            }
        }

        #endregion

        #region Fields

        private VideoView _videoView;
        private MediaController _mediaController;
        private MediaPlayer _mediaPlayer;
        private bool _isPrepared;

        #endregion
    }
}
