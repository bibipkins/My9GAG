using Xamarin.Forms;

namespace My9GAG.Models
{
    public class TextPostView : PostView
    {
        #region Properties

        public string Text
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public override void GenerateView()
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
        public override void Load()
        {
            // Does not hold any resources
        }
        public override void Unload()
        {
            // Does not hold any resources
        }

        #endregion
    }
}
