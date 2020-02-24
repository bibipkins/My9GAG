using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace My9GAG.Views.CustomViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ZoomableImage : ContentView
	{
        #region Constructors

        public ZoomableImage(double width, double height)
		{
			InitializeComponent();

            double factor = 2;
            image = new Image();

            if (height > width * factor)
            {
                image.Aspect = Aspect.AspectFill;

                var scrollView = new ScrollView()
                {
                    Content = image
                };

                this.Content = scrollView;
            }
            else
            {
                image.Aspect = Aspect.AspectFit;
                this.Content = image;
            }
        }

        #endregion

        #region BindableProperties

        #region Source

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create("Source", typeof(string), typeof(ZoomableImage), Image.SourceProperty.DefaultValue,
                Image.SourceProperty.DefaultBindingMode,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
                {
                    if (bindable is ZoomableImage zoomableImage)
                    {
                        zoomableImage.image.Source = (string)newValue;
                    }
                });

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        #endregion

        #region Aspect

        public static readonly BindableProperty AspectProperty =
            BindableProperty.Create("Aspect", typeof(Aspect), typeof(ZoomableImage), Image.AspectProperty.DefaultValue,
                Image.AspectProperty.DefaultBindingMode,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
                {
                    if (bindable is ZoomableImage zoomableImage)
                    {
                        zoomableImage.image.Aspect = (Aspect)newValue;
                    }
                });

        public Aspect Aspect
        {
            get { return (Aspect)GetValue(AspectProperty); }
            set { SetValue(AspectProperty, value); }
        }

        #endregion

        #endregion

        #region Properties

        public double OriginalWidth
        {
            get;
            set;
        }
        public double OriginalHeight
        {
            get;
            set;
        }

        #endregion

        #region Handlers

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (!isZoomed)
            {
                return;
            }

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    var translationX = panX + e.TotalX;

                    if (Math.Abs(translationX) < ((image.Width * 2.5) / 2))
                    {
                        image.TranslationX = translationX;
                    }

                    var translationY = panY + e.TotalY;

                    if (Math.Abs(translationY) < ((image.Height * 2.5) / 2))
                    {
                        image.TranslationY = translationY;
                    }

                    break;
                case GestureStatus.Completed:
                    panX = image.TranslationX;
                    panY = image.TranslationY;
                    break;
            }
        }

        private void OnDoubleTapped(object sender, EventArgs e)
        {
            if (isZoomed)
            {
                image.ScaleTo(1, easing: Easing.CubicOut);
                image.TranslateTo(0, 0, easing: Easing.CubicInOut);
                panX = panY = 0;
                isZoomed = false;
            }
            else
            {
                image.ScaleTo(2, easing: Easing.CubicOut);
                isZoomed = true;
            }
        }

        #endregion

        #region Fields

        private double panX = 0.0;
        private double panY = 0.0;
        private bool isZoomed = false;
        Image image = null;

        #endregion
    }
}
