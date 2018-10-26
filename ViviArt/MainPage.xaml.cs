using System;
using System.IO;
using Plugin.Toasts;
using Xamarin.Forms;


namespace ViviArt
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            dbOriPath.Text = GlobalResources.Current.dbPath;
            dbCopyPath.Text = DependencyService.Get<IExternalDir>().GetDocumentPath(GlobalResources.Current.dbName);
        }

        async void Clicked_Export(object sender, System.EventArgs e)
        {
            var oriPath = GlobalResources.Current.dbPath;
            var backupPath = DependencyService.Get<IExternalDir>().GetDocumentPath(GlobalResources.Current.dbName);
            try
            {
                File.Copy(oriPath, backupPath, true);
                INotificationResult result = await DependencyService.Get<IToastNotificator>().Notify(new NotificationOptions()
                {
                    Title = "내보내기 성공",
                    Description = $"{backupPath}",
                });
            }
            catch (Exception ex)
            {
                INotificationResult result = await DependencyService.Get<IToastNotificator>().Notify(new NotificationOptions()
                {
                    Title = "내보내기 실패",
                    Description = $"{ex.Message}",
                });
            }
        }
        async void Clicked_Import(object sender, System.EventArgs e)
        {
            var oriPath = DependencyService.Get<IExternalDir>().GetDocumentPath(GlobalResources.Current.dbName);
            var backupPath = GlobalResources.Current.dbPath;

            try
            {
                File.Copy(oriPath, backupPath, true);
                INotificationResult result = await DependencyService.Get<IToastNotificator>().Notify(new NotificationOptions()
                {
                    Title = "가져오기 성공",
                    Description = $"{backupPath}",
                });    
            }
            catch (Exception ex)
            {
                INotificationResult result = await DependencyService.Get<IToastNotificator>().Notify(new NotificationOptions()
                {
                    Title = "가져오기 실패",
                    Description = $"{ex.Message}",
                });    
            }
        }
    }
}
