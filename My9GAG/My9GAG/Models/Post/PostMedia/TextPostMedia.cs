using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public class TextPostMedia : SimpleTextPostMedia, IPostMedia
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
