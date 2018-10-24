using System;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ViviArt
{
    public partial class App : Application
    {
        public static Page CustomPage = null;

        public App()
        {
            InitializeComponent();
            if (CustomPage == null)
            {
                MainPage = new LoadingPage();    
                Task.Run(async () => {
                    await Task.Delay(1000);
                    DependencyService.Get<ICloseApplication>().Close();
                });
            }
            else
            {
                MainPage = CustomPage; 
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
