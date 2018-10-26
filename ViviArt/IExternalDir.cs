using System;
namespace ViviArt
{
    public interface IExternalDir
    {
        string GetDocumentPath(string fileName);
    }
}
