using Android.Content;
using Android.Content.Res;
using My9GAG.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(DarkEntryRenderer))]

namespace My9GAG.Droid.Renderers
{
    public class DarkEntryRenderer : EntryRenderer
    {
        public DarkEntryRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.White);
                Control.SetHintTextColor(ColorStateList.ValueOf(Android.Graphics.Color.Gray));
                Control.SetTextColor(ColorStateList.ValueOf(Android.Graphics.Color.White));
            }
        }
    }
}
