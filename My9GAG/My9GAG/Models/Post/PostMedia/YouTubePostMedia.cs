using Xamarin.Forms;

namespace My9GAG.Models
{
    public class YouTubePostMedia : IPostMedia
    {
        #region Constructors

        public YouTubePostMedia(string url)
        {
            Url = url;
            var html = new HtmlWebViewSource
            {
                Html = GenerateHtml()
            };
            View = new WebView()
            {
                Source = html
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
            get { return PostType.Video; }
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

        #region Implementation

        private string GenerateHtml()
        {
            return "<iframe width=\"100%\" height=\"100%\" src=\"https://www.youtube.com/embed/" + Url +
                "\" frameborder = \"0\" allow = \"accelerometer;\" allowfullscreen></iframe>";
        }

        #endregion
    }
}
