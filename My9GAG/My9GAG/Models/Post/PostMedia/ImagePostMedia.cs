using My9GAG.Views;
using Xamarin.Forms;

namespace My9GAG.Models
{
    public class ImagePostMedia : IPostMedia
    {
        #region Constructors

        public ImagePostMedia(string url)
        {
            Url = url;
            View = new ZoomableImage()
            {
                Source = Url
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
            get { return PostType.Photo; }
        }

        #endregion

        #region Methods

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
