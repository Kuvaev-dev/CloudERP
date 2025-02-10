using System.IO;

namespace Utils.Interfaces
{
    public interface IFile
    {
        string FileName { get; }
        int ContentLength { get; }
        Stream InputStream { get; }
        void SaveAs(string path);
    }
}
