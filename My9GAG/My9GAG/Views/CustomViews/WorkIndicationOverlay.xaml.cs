using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace My9GAG.Views.CustomViews
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WorkIndicationOverlay : ContentView
	{
        #region Constructors

        public WorkIndicationOverlay()
		{
			InitializeComponent();
		}

        #endregion

        #region BindableProperties

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            "Text",
            typeof(string),
            typeof(WorkIndicationOverlay),
            Label.TextProperty.DefaultValue,
            Label.TextProperty.DefaultBindingMode,
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
            {
                if (bindable is WorkIndicationOverlay workOverlay)
                {
                    workOverlay.label.Text = (string)newValue;
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