using System.Web;

namespace Domain.Services
{
    public interface IFileService
    {
        string UploadPhoto(HttpPostedFileBase file, string folderPath, string fileName);
        string SetDefaultPhotoPath(string defaultPath);
    }
}
