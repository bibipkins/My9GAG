using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Plugin.Permissions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace My9GAG.Droid
{
    [Activity(
        Label = "My9GAG", 
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

            Platform.Init(this, bundle);

            Forms.SetFlags("CollectionView_Experimental");
            Forms.Init(this, bundle);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
