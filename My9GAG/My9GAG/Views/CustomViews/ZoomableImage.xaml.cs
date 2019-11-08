using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace My9GAG.Views.CustomViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ZoomableImage : ContentView
	{
		public ZoomableImage()
		{
			InitializeComponent();
        }

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create("Source", typeof(string), typeof(ZoomableImage), Image.SourceProperty.DefaultValue,
                Image.SourceProperty.DefaultBindingMode,
                propertyChanged: (BindableObject bindable, object oldValue, object newValue) =>
                {
                    var zoomableImage = bindable as ZoomableImage;

                    if (zoomableImage == null)
                        return;

                    zoomableImage.image.Source = (string)newValue;
                });

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the X pixel coordinate.
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the Y pixel coordinate.
                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.
                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                // Apply translation based on the change in origin.
                Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
                Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

                Content.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                if (Scale > maxScale)
                {
                    Debug.WriteLine("-----" + Scale + "-----");
                    this.ScaleTo(startScale, 250, Easing.SpringOut);
                    return;
                }

                // Store the translation delta's of the wrapped user interface element.
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }
        }

        private double currentScale = 1;
        private double startScale = 1;
        private double maxScale = 2;
        private double xOffset = 0;
        private double yOffset = 0;
    }

    public static class DoubleExtensions
    {
        public static double Clamp(this double self, double min, double max)
        {
            return Math.Min(max, Math.Max(self, min));
        }
    }
}
