using Xamarin.Forms;

namespace My9GAG.Models
{
    public class TextPostMedia : IPostMedia
    {
        #region Constructors

        public TextPostMedia(string text)
        {
            Url = "";
            Text = text;
            View = new Label()
            {
                Text = Text,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };
        }

        #endregion

        #region Properties

        public string Text
        {
            get;
            set;
        }
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
            get { return PostType.Other; }
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
