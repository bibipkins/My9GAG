using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public class YouTubePostMedia : SimpleYoutubePostMedia, IPostMedia
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
            View = new WebView()
            {
                Source = new HtmlWebViewSource
                {
                    Html = GenerateHtml()
                }
            };
        }
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
        public void Unload()
        {

        }

        #endregion

        #region Implementation

        private string GenerateHtml()
        {
            return "<html><head><style>body,html{background:black;margin:0;}</style></head>" +
                "<body><iframe width=\"100%\" height=\"100%\" src=\"https://www.youtube.com/embed/" + Url +
                "\" frameborder = \"0\" allow = \"accelerometer;\" allowfullscreen></iframe></body></html>";
        }

        #endregion
    }
}
