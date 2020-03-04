using My9GAG.Views.CustomViews.VideoPlayer;
using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public class AnimatedPostMedia : IPostMedia
    {
        #region Properties

        public View View
        {
            get;
            protected set;
        }
        public PostType Type
        {
            get { return PostType.Animated; }
        }
        public string Url
        {
            get;
            set;
        }
        public double Width 
        { 
            get;
            set;
        }
        public double Height 
        { 
            get;
            set;
        }

        #endregion

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
