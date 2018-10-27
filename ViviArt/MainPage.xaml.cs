using System;
using System.IO;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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

            float fontSize = float.Parse(Setting.ValueOf(SettingKey.MandalaHomeWidgetFontSize) ?? "1.0");
            fontSizeText.Text = $"{fontSize*100:F0} %";
        }

        void Clicked_FontSizeBefore(object sender, EventArgs e)
        {
            float fontSize = float.Parse(Setting.ValueOf(SettingKey.MandalaHomeWidgetFontSize) ?? "1.0");
            fontSize -= 0.1f;
            Setting.Save(SettingKey.MandalaHomeWidgetFontSize, $"{fontSize:F1}");
            fontSizeText.Text = $"{fontSize * 100:F0} %";
            DependencyService.Get<IHomeWidget>().RefreshFontSize();
        }
        void Clicked_FontSizeNext(object sender, EventArgs e)
        {
            float fontSize = float.Parse(Setting.ValueOf(SettingKey.MandalaHomeWidgetFontSize) ?? "1.0");
            fontSize += 0.1f;
            Setting.Save(SettingKey.MandalaHomeWidgetFontSize, $"{fontSize:F1}");

            fontSizeText.Text = $"{fontSize * 100:F0} %";
            DependencyService.Get<IHomeWidget>().RefreshFontSize();
        }

        async void Clicked_Export(object sender, System.EventArgs e)
        {
            await CheckLocationPermission();

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
            await CheckLocationPermission();

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
        public async Task CheckLocationPermission()
        {
            // 권한요청 
            var status = PermissionStatus.Unknown;
            status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (status != PermissionStatus.Granted)
            {
                status = await PermissionUtil.CheckPermissions(Permission.Storage);
            }
        }
    }
}
