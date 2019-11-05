using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace My9GAG.Views.CustomViews.Overlays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MessageOverlay : ContentView
	{
        #region Constructors

        public MessageOverlay()
		{
			InitializeComponent();
		}

        #endregion

        #region BindableProperties

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            "Text", 
            typeof(string), 
            typeof(MessageOverlay),
            Label.TextProperty.DefaultValue,
            Label.TextProperty.DefaultBindingMode,
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
            {
                if (bindable is MessageOverlay messageOverlay)
                {
                    messageOverlay.label.Text = (string)newValue;
                }
            });

        #endregion

        #region Properties

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion
    }
}
