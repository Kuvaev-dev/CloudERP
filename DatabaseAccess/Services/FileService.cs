using Domain.Services;
using System;
using System.IO;
using System.Web;

namespace DatabaseAccess.Services
{
    public class FileService : IFileService
    {
        public string UploadPhoto(IFile file, string folderPath, string fileName)
        {
            if (file == null || file.ContentLength <= 0)
                return null;

            var fullPath = Path.Combine(HttpContext.Current.Server.MapPath(folderPath), fileName);

            try
            {
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                file.SaveAs(fullPath);

                return $"{folderPath}/{fileName}".Replace("~", string.Empty);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string SetDefaultPhotoPath(string defaultPath)
        {
            return string.IsNullOrEmpty(defaultPath)
                ? throw new Exception("Default Photo Path Not Found")
                : defaultPath;
        }
    }
}
