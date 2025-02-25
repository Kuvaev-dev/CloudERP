using Utils.Interfaces;

namespace Domain.ServiceAccess  
{
    public interface IFileService
    {
        Task<string?> UploadPhotoAsync(IFile file, string folderPath, string fileName);
        string SetDefaultPhotoPath(string defaultPath);
    }
}
