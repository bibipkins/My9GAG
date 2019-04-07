using My9GAG.Views;
using Xamarin.Forms;

namespace My9GAG.Models
{
    public class AnimatedPostMedia : IPostMedia
    {
        #region Constructors

        public AnimatedPostMedia(string url)
        {
            Url = url;
            View = new VideoPlayer();
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

        public void Pause()
        {
            
        }
        public void Reload()
        {
            
        }
        public void Start()
        {
            
        }
        public void Stop()
        {
            
        }

        #endregion
    }
}
