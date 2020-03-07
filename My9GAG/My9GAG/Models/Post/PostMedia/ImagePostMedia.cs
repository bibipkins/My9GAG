using My9GAG.Views.CustomViews;
using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public class ImagePostMedia : SimpleImagePostMedia, IPostMedia
    {
        #region Properties

        public View View
        {
            get;
            protected set;
        }
        #endregion

        #region Methods

        public void GenerateView()
        {
            View = new ZoomableImage(Width, Height)
            {
                Source = Url
            };
        }
        public void Start()
        {
            // No implementation
        }
        public void Stop()
        {
            // No implementation
        }
        public void Pause()
        {
            // No implementation
        }
        public void Reload()
        {
            (View as ZoomableImage).Source = Url;
        }
        public void Unload()
        {
            (View as ZoomableImage).Source = null;
        }

        #endregion
    }
}
