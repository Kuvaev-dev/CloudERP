namespace Domain.Interfaces
{
    public interface IFileService
    {
        string UploadPhoto(IFile file, string folderPath, string fileName);
        string SetDefaultPhotoPath(string defaultPath);
    }
}
