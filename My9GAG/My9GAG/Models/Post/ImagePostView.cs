using My9GAG.Views.CustomViews;
using NineGagApiClient.Models;

namespace My9GAG.Models
{
    public class ImagePostView : PostView
    {
        #region Methods

        public override void GenerateView()
        {
            View = new ZoomableImage(PostMedia.Width, PostMedia.Height)
            {
                Source = PostMedia.Url
            };
        }
        public override void Load()
        {
            (View as ZoomableImage).Source = Url;
        }
        public override void Unload()
        {
            (View as ZoomableImage).Source = null;
        }

        #endregion
    }
}
