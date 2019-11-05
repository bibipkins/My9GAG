using System;
using Xamarin.Forms;

namespace My9GAG.Views.CustomViews.VideoPlayer
{
    public class VideoPlayer : View, IVideoPlayer
    {
        #region Constructors

        public VideoPlayer()
        {
            UpdateStatus?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        public event EventHandler UpdateStatus;

        public event EventHandler PlayRequested;

        public event EventHandler PauseRequested;

        public event EventHandler StopRequested;

        #endregion

        #region BindableProperties

        #region AreTransportControlsEnabled

        public static readonly BindableProperty AreTransportControlsEnabledProperty =
            BindableProperty.Create(nameof(AreTransportControlsEnabled), typeof(bool), typeof(VideoPlayer), true);

        public bool AreTransportControlsEnabled
        {
            set { SetValue(AreTransportControlsEnabledProperty, value); }
            get { return (bool)GetValue(AreTransportControlsEnabledProperty); }
        }

        #endregion

        #region Source

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(VideoSource), typeof(VideoPlayer), null);

        [TypeConverter(typeof(VideoSourceConverter))]
        public VideoSource Source
        {
            set { SetValue(SourceProperty, value); }
            get { return (VideoSource)GetValue(SourceProperty); }
        }

        #endregion Source

        #region AutoPlay

        public static readonly BindableProperty AutoPlayProperty =
            BindableProperty.Create(nameof(AutoPlay), typeof(bool), typeof(VideoPlayer), false);

        public bool AutoPlay
        {
            set { SetValue(AutoPlayProperty, value); }
            get { return (bool)GetValue(AutoPlayProperty); }
        }

        #endregion

        #region Status

        private static readonly BindablePropertyKey StatusPropertyKey =
            BindableProperty.CreateReadOnly(nameof(Status), typeof(VideoPlayerStatus), typeof(VideoPlayer), VideoPlayerStatus.Loading);

        public static readonly BindableProperty StatusProperty = StatusPropertyKey.BindableProperty;

        public VideoPlayerStatus Status
        {
            get { return (VideoPlayerStatus)GetValue(StatusProperty); }
        }

        VideoPlayerStatus IVideoPlayer.Status
        {
            set { SetValue(StatusPropertyKey, value); }
            get { return Status; }
        }

        #endregion

        #region Duration

        private static readonly BindablePropertyKey DurationPropertyKey =
            BindableProperty.CreateReadOnly(nameof(Duration), typeof(TimeSpan), typeof(VideoPlayer), new TimeSpan(),
                propertyChanged: (bindable, oldValue, newValue) => ((VideoPlayer)bindable).SetTimeToEnd());

        public static readonly BindableProperty DurationProperty = DurationPropertyKey.BindableProperty;

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
        }

        TimeSpan IVideoPlayer.Duration
        {
            set { SetValue(DurationPropertyKey, value); }
            get { return Duration; }
        }

        #endregion

        #region Position

        public static readonly BindableProperty PositionProperty =
            BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(VideoPlayer), new TimeSpan(),
                propertyChanged: (bindable, oldValue, newValue) => ((VideoPlayer)bindable).SetTimeToEnd());

        public TimeSpan Position
        {
            set { SetValue(PositionProperty, value); }
            get { return (TimeSpan)GetValue(PositionProperty); }
        }

        #endregion

        #region TimeToEnd

        private static readonly BindablePropertyKey TimeToEndPropertyKey =
            BindableProperty.CreateReadOnly(nameof(TimeToEnd), typeof(TimeSpan), typeof(VideoPlayer), new TimeSpan());

        public static readonly BindableProperty TimeToEndProperty = TimeToEndPropertyKey.BindableProperty;

        public TimeSpan TimeToEnd
        {
            private set { SetValue(TimeToEndPropertyKey, value); }
            get { return (TimeSpan)GetValue(TimeToEndProperty); }
        }

        #endregion

        #endregion

        #region Methods

        public void Play()
        {
            PlayRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Pause()
        {
            PauseRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            StopRequested?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Implementation

        void SetTimeToEnd()
        {
            TimeToEnd = Duration - Position;
        }

        #endregion
    }
}
