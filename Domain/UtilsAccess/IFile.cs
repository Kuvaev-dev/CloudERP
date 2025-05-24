namespace Domain.UtilsAccess
{
    public interface IFile
    {
        string FileName { get; }
        long ContentLength { get; }
        Stream InputStream { get; }
        void SaveAs(string path);
    }
}
