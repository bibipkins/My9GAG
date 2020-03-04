using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WebView), typeof(My9GAG.Droid.Renderers.DesktopWebViewRenderer))]

namespace My9GAG.Droid.Renderers
{
    public class DesktopWebViewRenderer : WebViewRenderer
    {
        public DesktopWebViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);
            Control.Settings.UserAgentString = "Mozilla/5.0 Google";
        }
    }
}
