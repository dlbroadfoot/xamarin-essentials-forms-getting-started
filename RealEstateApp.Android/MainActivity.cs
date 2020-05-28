using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Xamarin.Essentials;
using RealEstateApp.ViewModels;
using Android.Support.Design.Widget;
using Android.Util;

namespace RealEstateApp.Droid
{
    [Activity(Label = "RealEstate", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.FullSensor)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }
    }
}