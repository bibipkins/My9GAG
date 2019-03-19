using Rox;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace My9GAG.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VideoPlayer : ContentView
	{
		public VideoPlayer ()
		{
			InitializeComponent ();

            player.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "VideoState")
                {
                    if (player.VideoState == VideoStateType.Stopped)
                    {
                        playImage.IsVisible = true;
                        pauseImage.IsVisible = false;
                    }
                }
            };
		}

        public void Start()
        {
            player.Start();
            playImage.IsVisible = false;
            pauseImage.IsVisible = true;
        }
        public void Pause()
        {
            player.Pause();
            playImage.IsVisible = true;
            pauseImage.IsVisible = false;
        }
        public void Stop()
        {
            player.Stop();
            playImage.IsVisible = true;
            pauseImage.IsVisible = false;
        }
        public void VolumeOn()
        {
            player.Muted = true;
            volumeMutedImage.IsVisible = true;
            volumeOnImage.IsVisible = false;
        }
        public void VolumeMute()
        {
            player.Muted = false;
            volumeMutedImage.IsVisible = false;
            volumeOnImage.IsVisible = true;
        }

        public static readonly BindableProperty SourceProperty = 
            BindableProperty.Create("Source", typeof(string), typeof(VideoPlayer), VideoView.SourceProperty.DefaultValue,
                VideoView.SourceProperty.DefaultBindingMode, 
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) => 
                {
                    VideoPlayer videoPlayer = bindable as VideoPlayer;

                    if (videoPlayer == null)
                        return;

                    videoPlayer.player.Source = (string)newValue;
                });

        public string Source
        {
            get
            {
                return (string)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        private void player_Tapped(object sender, EventArgs e)
        {
            switch (player.VideoState)
            {
                case Rox.VideoStateType.Playing:
                    Pause();
                    break;
                case Rox.VideoStateType.Paused:
                    Start();
                    break;
                case Rox.VideoStateType.Stopped:
                    Start();
                    break;
            }
        }

        private void volumeOnImage_Tapped(object sender, EventArgs e)
        {
            VolumeOn();
        }
        private void volumeMutedImage_Tapped(object sender, EventArgs e)
        {
            VolumeMute();
        }
        private void playImage_Tapped(object sender, EventArgs e)
        {
            Start();
        }
        private void pauseImage_Tapped(object sender, EventArgs e)
        {
            Pause();
        }
        private void stopImage_Tapped(object sender, EventArgs e)
        {
            Stop();
        }
    }
}