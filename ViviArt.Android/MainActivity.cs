using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Content;
using System;
using Android.Preferences;
using Xamarin.Forms;
using Plugin.Toasts;

namespace ViviArt.Droid
{
    [Activity(Label = "ViviArt", Icon = "@mipmap/launcher_foreground", Theme = "@style/MainTheme", MainLauncher = true, 
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        static int PICK_CONTACT_REQUEST = 0;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);
            Instance = this;
            //toast plugin setting
            DependencyService.Register<ToastNotification>();
            ToastNotification.Init(this);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

