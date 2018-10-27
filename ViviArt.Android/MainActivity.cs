using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Content;
using System;
using Android.Preferences;
using Xamarin.Forms;
using Plugin.Toasts;
using Plugin.Permissions;

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
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            switch (Intent.Action)
            {
                case TodoProvider.OPEN_TODO_EDIT:
                    {
                        var newPage = new TodoItemEdit();
                        newPage.viewModel.ItemID = Intent.GetIntExtra(TodoProvider.EXTRA_ID, -1);
                        Xamarin.Forms.Application.Current.MainPage = newPage;
                        break;
                    }
                case TodoProvider.OPEN_TODO_ADD:
                    {
                        Xamarin.Forms.Application.Current.MainPage = new TodoItemEdit();
                        break;
                    }
                case MandalaArtProvider.OPEN_MANDALA_CORE_ADD:
                    {
                        var newPage = new MandalaCoreEdit();
                        newPage.viewModel.MyItem.Position = Intent.GetIntExtra(MandalaArtProvider.EXTRA_CORE_POSITION, -1);
                        Xamarin.Forms.Application.Current.MainPage = newPage;
                        break;
                    }
                case MandalaArtProvider.OPEN_MANDALA_CORE_EDIT:
                    {
                        var newPage = new MandalaCoreEdit();
                        newPage.viewModel.ItemID = Intent.GetIntExtra(MandalaArtProvider.EXTRA_CORE_ID, -1);
                        Xamarin.Forms.Application.Current.MainPage = newPage;
                        break;
                    }
                case MandalaArtProvider.OPEN_MANDALA_MIDDLE_ADD:
                    {
                        var newPage = new MandalaMiddleEdit();
                        newPage.viewModel.MyItem.CoreGoalID = Intent.GetIntExtra(MandalaArtProvider.EXTRA_CORE_ID, -1);
                        newPage.viewModel.MyItem.Position = Intent.GetIntExtra(MandalaArtProvider.EXTRA_MIDDLE_POSITION, -1);
                        Xamarin.Forms.Application.Current.MainPage = newPage;
                        break;
                    }
                case MandalaArtProvider.OPEN_MANDALA_MIDDLE_EDIT:
                    {
                        var newPage = new MandalaMiddleEdit();
                        newPage.viewModel.ItemID = Intent.GetIntExtra(MandalaArtProvider.EXTRA_MIDDLE_ID, -1);
                        Xamarin.Forms.Application.Current.MainPage = newPage;
                        break;
                    }
                case MandalaArtProvider.OPEN_MANDALA_CORE_CHART:
                    {
                        Console.WriteLine($"today {Intent.GetStringExtra(MandalaArtProvider.EXTRA_TODAY)}");
                        var newPage = new MandalaCoreChart();
                        var myInput = new MandalaCoreChartInput()
                        {
                            CoreGoalID = Intent.GetIntExtra(MandalaArtProvider.EXTRA_CORE_ID, -1),
                            Today = Intent.GetStringExtra(MandalaArtProvider.EXTRA_TODAY).ToDateTime2()
                        };
                        newPage.viewModel.InputSet = myInput;
                        Xamarin.Forms.Application.Current.MainPage = newPage;
                        break;
                    }
                case MandalaArtProvider.OPEN_MANDALA_MIDDLE_CHART:
                    {
                        Console.WriteLine($"today {Intent.GetStringExtra(MandalaArtProvider.EXTRA_TODAY)}");
                        var newPage = new MandalaMiddleChart();
                        var myInput = new MandalaMiddleChartInput()
                        {
                            MiddleGoalID = Intent.GetIntExtra(MandalaArtProvider.EXTRA_MIDDLE_ID, -1),
                            Today = Intent.GetStringExtra(MandalaArtProvider.EXTRA_TODAY).ToDateTime2()
                        };
                        newPage.viewModel.InputSet = myInput;
                        Xamarin.Forms.Application.Current.MainPage = newPage;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}

