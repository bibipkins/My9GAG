using My9GAG.Views.CustomViews.VideoPlayer;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using MediaElement = Windows.UI.Xaml.Controls.MediaElement;
using MediaElementState = Windows.UI.Xaml.Media.MediaElementState;

[assembly: ExportRenderer(typeof(VideoPlayerControl), typeof(My9GAG.UWP.Renderers.VideoPlayerRenderer))]

namespace My9GAG.UWP.Renderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayerControl, MediaElement>
    {
        #region Handlers

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayerControl> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    var mediaElement = new MediaElement();
                    SetNativeControl(mediaElement);

                    mediaElement.MediaOpened += OnMediaElementMediaOpened;
                    mediaElement.CurrentStateChanged += OnMediaElementCurrentStateChanged;
                }

                SetIsMuted();
                SetAreTransportControlsEnabled();
                SetSource();
                SetAutoPlay();

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
            else if (args.PropertyName == VideoPlayerControl.AutoPlayProperty.PropertyName)
            {
                SetAutoPlay();
            }
            else if (args.PropertyName == VideoPlayerControl.PositionProperty.PropertyName)
            {
                if (Math.Abs((Control.Position - Element.Position).TotalSeconds) > 1)
                {
                    Control.Position = Element.Position;
                }
            }
            else if (args.PropertyName == VideoPlayerControl.IsMutedProperty.PropertyName)
            {
                SetIsMuted();
            }
        }

        void OnMediaElementMediaOpened(object sender, RoutedEventArgs args)
        {
            ((IVideoPlayer)Element).Duration = Control.NaturalDuration.TimeSpan;
        }

        void OnMediaElementCurrentStateChanged(object sender, RoutedEventArgs args)
        {
            VideoPlayerStatus videoStatus = VideoPlayerStatus.Loading;

            switch (Control.CurrentState)
            {
                case MediaElementState.Playing:
                    videoStatus = VideoPlayerStatus.Playing;
                    break;
                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                    videoStatus = VideoPlayerStatus.Paused;
                    break;
            }

            ((IVideoPlayer)Element).Status = videoStatus;
        }

        void OnUpdateStatus(object sender, EventArgs args)
        {
            ((IElementController)Element).SetValueFromRenderer(VideoPlayerControl.PositionProperty, Control.Position);
        }

        void OnPlayRequested(object sender, EventArgs args)
        {
            Control.Play();
        }

        void OnPauseRequested(object sender, EventArgs args)
        {
            Control.Pause();
        }

        void OnStopRequested(object sender, EventArgs args)
        {
            Control.Stop();
        }

        #endregion

        #region Implementation

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                Control.MediaOpened -= OnMediaElementMediaOpened;
                Control.CurrentStateChanged -= OnMediaElementCurrentStateChanged;
            }

            base.Dispose(disposing);
        }

        private void SetAreTransportControlsEnabled()
        {
            Control.AreTransportControlsEnabled = Element.AreTransportControlsEnabled;
        }
        private void SetSource()
        {
            bool hasSetSource = false;

            if (Element.Source is UriVideoSource)
            {
                string uri = (Element.Source as UriVideoSource).Uri;

                if (!string.IsNullOrWhiteSpace(uri))
                {
                    Control.Source = new Uri(uri);
                    hasSetSource = true;
                }
            }

            if (!hasSetSource)
            {
                Control.Source = null;
            }
        }
        private void SetAutoPlay()
        {
            Control.AutoPlay = Element.AutoPlay;
        }
        private void SetIsMuted()
        {
            Control.IsMuted = Element.IsMuted;
        }

        #endregion
    }
}
