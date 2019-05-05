using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using PanCardView.Droid;

namespace My9GAG.Droid
{
    [Activity(Label = "My9GAG", 
        Icon = "@mipmap/icon", 
        Theme = "@style/SplashTheme",
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            CardsViewRenderer.Preserve();
            CarouselView.FormsPlugin.Android.CarouselViewRenderer.Init();
            LoadApplication(new App());
        }
    }
}
