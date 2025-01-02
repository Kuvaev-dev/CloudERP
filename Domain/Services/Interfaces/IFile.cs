using System.IO;

namespace Domain.Services
{
    public interface IFile
    {
        string FileName { get; }
        int ContentLength { get; }
        Stream InputStream { get; }
        void SaveAs(string path);
    }
}
