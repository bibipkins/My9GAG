using My9GAG.Views.CustomViews.VideoPlayer;
using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public class AnimatedPostMedia : SimpleAnimatedPostMedia, IPostMedia
    {
        public View View
        {
            get;
            protected set;
        }

        #region Methods

        public void GenerateView()
        {
            View = new VideoPlayer()
            {
                Source = new UriVideoSource()
                {
                    Uri = Url
                }
            };
        }
        public void Start()
        {
            (View as VideoPlayer).Play();
        }
        public void Stop()
        {
            (View as VideoPlayer).Stop();
        }
        public void Pause()
        {
            (View as VideoPlayer).Pause();
        }
        public void Reload()
        {
            GenerateView();
        }
        public void Unload()
        {
            View = null;
        }
        
        #endregion
    }
}
