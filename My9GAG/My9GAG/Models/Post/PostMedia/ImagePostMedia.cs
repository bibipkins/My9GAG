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

        public void Pause()
        {

        }
        public void Reload()
        {
            View = new ZoomableImage()
            {
                Source = Url
            };
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
