using System.IO;
using Android.Content;
using Xamarin.Forms;

[assembly: Dependency(typeof(ViviArt.Droid.HomeWidget))]
namespace ViviArt.Droid
{
    public class HomeWidget : IHomeWidget
    {
        public void ImportMandalaGoal()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MandalaArtProvider));
            intent.SetAction(MandalaArtProvider.ACTION_MANDALA_IMPORT);
            Android.App.Application.Context.SendBroadcast(intent);
        }

        public void RefreshFontSize()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MandalaArtProvider));
            intent.SetAction(MandalaArtProvider.ACTION_MANDALA_REFRESH_FONTSIZE);
            Android.App.Application.Context.SendBroadcast(intent);
        }

    }
}
