using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public class TextPostMedia : IPostMedia
    {
        #region Properties

        public View View
        {
            get;
            protected set;
        }
        public PostType Type
        {
            get { return PostType.Other; }
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
        public string Text
        {
            get;
            set;
        }
        
        #endregion

        #region Methods

        public void GenerateView()
        {
            View = new Label()
            {
                Text = Text,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Black,
                TextColor = Color.White
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
    }
}
