using System;
using Xamarin.Forms;

namespace My9GAG.Views.CustomViews.VideoPlayer
{
    public class VideoSourceConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return VideoSource.FromUri(value);
            }

            throw new InvalidOperationException("Cannot convert null or whitespace to ImageSource");
        }
    }
}
