using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(ViviArt.Droid.ExternalDir))]
namespace ViviArt.Droid
{
    public class ExternalDir : IExternalDir
    {
        public string GetDocumentPath(string fileName)
        {
            return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads, fileName);
        }
    }
}
