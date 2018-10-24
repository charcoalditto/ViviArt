using System;
using Android.App;
using Xamarin.Forms;

[assembly: Dependency(typeof(ViviArt.Droid.CloseApplication))]
namespace ViviArt.Droid
{
    public class CloseApplication : ICloseApplication
    {
        public void Close()
        {
            var activity = (Activity)Forms.Context;
            activity.FinishAffinity();    
        }
    }
}
