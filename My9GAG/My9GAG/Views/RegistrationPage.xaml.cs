using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace My9GAG.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegistrationPage : ContentPage
	{
		public RegistrationPage()
		{
			InitializeComponent();
		}

        private void OnLayoutSizeChanged(object sender, EventArgs e)
        {
            if (Device.Idiom != TargetIdiom.Phone)
            {
                if (Width > MIN_WIDTH_TO_CLAMP)
                {
                    AbsoluteLayout.SetLayoutBounds(layout, new Rectangle(0.5, 0.5, 0.5, 1));
                    layout.VerticalOptions = LayoutOptions.CenterAndExpand;
                }
                else
                {
                    AbsoluteLayout.SetLayoutBounds(layout, new Rectangle(0, 0, 1, 1));
                    layout.VerticalOptions = LayoutOptions.FillAndExpand;
                }
            }
        }

        private const double MIN_WIDTH_TO_CLAMP = 720.0;
    }
}
