using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace My9GAG.Views.CustomViews.VideoPlayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPlayer : ContentView
    {
        #region Constructors

        public VideoPlayer()
        {
            InitializeComponent();
            videoPlayerControl.PropertyChanged += OnVideoPlayerPropertyChanged;
        }

        #endregion

        #region Bindable Properties

        #region Source

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(VideoSource), typeof(VideoPlayer), null,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
                {
                    if (bindable is VideoPlayer videoPlayer)
                    {
                        videoPlayer.videoPlayerControl.Source = (VideoSource)newValue;
                    }
                });

        [Xamarin.Forms.TypeConverter(typeof(VideoSourceConverter))]
        public VideoSource Source
        {
            set { SetValue(SourceProperty, value); }
            get { return (VideoSource)GetValue(SourceProperty); }
        }

        #endregion Source

        #endregion

        #region Properties

        public bool IsLoading
        {
            get;
            private set;
        }
        public bool IsPlaying
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public void Play()
        {
            videoPlayerControl.Play();
        }
        public void Pause()
        {
            videoPlayerControl.Pause();
        }
        public void Stop()
        {
            videoPlayerControl.Pause();
            videoPlayerControl.Position = new TimeSpan();
        }
        public void Mute()
        {
            videoPlayerControl.IsMuted = true;
        }
        public void UnMute()
        {
            videoPlayerControl.IsMuted = false;
        }

        #endregion

        #region Handlers

        private void OnVideoPlayerPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == VideoPlayerControl.StatusProperty.PropertyName)
            {
                SetStatus();
            }
            else if (args.PropertyName == VideoPlayerControl.DurationProperty.PropertyName)
            {
                SetDuration();
            }
            else if (args.PropertyName == VideoPlayerControl.PositionProperty.PropertyName)
            {
                SetPosition();
            }
            else if (args.PropertyName == VideoPlayerControl.IsMutedProperty.PropertyName)
            {
                SetIsMuted();
            }
        }

        private void OnPlayerTapped(object sender, EventArgs args)
        {
            switch (videoPlayerControl.Status)
            {
                case VideoPlayerStatus.Playing:
                    Pause();
                    break;
                case VideoPlayerStatus.Paused:
                    Play();
                    break;
                default:
                    break;
            }
        }

        private void OnVolumeOnImageTapped(object sender, EventArgs args)
        {
            Mute();
        }
        private void OnVolumeOffImageTapped(object sender, EventArgs args)
        {
            UnMute();
        }
        private void OnPlayImageTapped(object sender, EventArgs args)
        {
            Play();
        }
        private void OnPauseImageTapped(object sender, EventArgs args)
        {
            Pause();
        }
        private void OnStopImageTapped(object sender, EventArgs args)
        {
            Stop();
        }

        private void OnTimelineValueChanged(object sender, ValueChangedEventArgs args)
        {
            double lowerBound = args.OldValue - 50;
            double upperBound = args.OldValue + 50;

            if (args.NewValue < lowerBound || args.NewValue > upperBound)
            {
                videoPlayerControl.Position = TimeSpan.FromMilliseconds(args.NewValue);
            }
        }

        #endregion

        #region Implementation

        private void SetStatus()
        {
            IsLoading = videoPlayerControl.Status == VideoPlayerStatus.Loading;
            IsPlaying = videoPlayerControl.Status == VideoPlayerStatus.Playing;

            controlsOverlay.IsVisible = !IsLoading;
            loadingOverlay.IsVisible = IsLoading;
            loadingIndicator.IsVisible = IsLoading;
            loadingIndicator.IsRunning = IsLoading;

            playImage.IsVisible = !IsPlaying;
            pauseImage.IsVisible = IsPlaying;
        }
        private void SetDuration()
        {
            var duration = videoPlayerControl.Duration;

            timelineSlider.Maximum = duration.TotalMilliseconds;
            timelineSlider.Minimum = 0;

            endLabel.Text = GetFormattedTime(duration);
        }
        private void SetPosition()
        {
            timelineSlider.Value = videoPlayerControl.Position.TotalMilliseconds;
            startLabel.Text = GetFormattedTime(videoPlayerControl.Position);
        }
        private void SetIsMuted()
        {
            volumeMutedImage.IsVisible = videoPlayerControl.IsMuted;
            volumeOnImage.IsVisible = !videoPlayerControl.IsMuted;
        }

        private string GetFormattedTime(TimeSpan timeSpan)
        {
            string formatString = timeSpan.TotalHours >= 1 ? "hh':'mm':'ss" : "mm':'ss";
            return timeSpan.ToString(formatString);
        }

        #endregion
    }
}
