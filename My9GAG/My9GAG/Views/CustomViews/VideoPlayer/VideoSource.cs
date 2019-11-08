using Xamarin.Forms;

namespace My9GAG.Views.CustomViews.VideoPlayer
{
    [TypeConverter(typeof(VideoSourceConverter))]
    public class VideoSource : Element
    {
        public static VideoSource FromUri(string uri)
        {
            return new UriVideoSource() { Uri = uri };
        }
    }
}
