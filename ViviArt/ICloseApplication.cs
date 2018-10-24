using System;
namespace ViviArt
{
    public interface ICloseApplication
    {
        void Close();
    }
}


/*

down vote
If you are using Xamarin.Forms create a Dependency Service.

Interface

public interface ICloseApplication
{
    void closeApplication();
}
Android : Using FinishAffinity() won't restart your activity. It will simply close the application.

public class CloseApplication : ICloseApplication
{
    public void closeApplication()
    {
        var activity = (Activity)Forms.Context;
        activity.FinishAffinity();
    }
}
IOS : As already suggested above.

public class CloseApplication : ICloseApplication
{
    public void closeApplication()
    {
        Thread.CurrentThread.Abort();
    }
}
UWP

public class CloseApplication : ICloseApplication
{
    public void closeApplication()
    {
        Application.Current.Exit();
    }
}
Usage in Xamarin Forms

var closer = DependencyService.Get<ICloseApplication>();
    closer?.closeApplication();

*/