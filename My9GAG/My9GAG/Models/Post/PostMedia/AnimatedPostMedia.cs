using My9GAG.Views.CustomViews;
using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public class AnimatedPostMedia : IPostMedia
    {
        #region Constructors

        public AnimatedPostMedia(string url)
        {
            Url = url;
            View = new VideoPlayer()
            {
                Source = url
            };
        }

        #endregion

        #region Properties

        public string Url
        {
            get;
            set;
        }
        public View View
        {
            get;
            protected set;
        }
        public PostType Type
        {
            get { return PostType.Animated; }
        }

        #endregion

        #region Methods

        public void Start()
        {
            (View as VideoPlayer).Start();
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
            (View as VideoPlayer).Source = Url;
        }
        public void Unload()
        {

        }

        #endregion
    }
}
