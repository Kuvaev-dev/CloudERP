using System;
using System.IO;
using System.Web;
using Utils.Interfaces;

namespace Services.Implementations
{
    public class FileService : IFileService
    {
        public string UploadPhoto(IFile file, string folderPath, string fileName)
        {
            if (file?.ContentLength <= 0) return null;

            var fullPath = Path.Combine(HttpContext.Current.Server.MapPath(folderPath), fileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) 
                    ?? throw new InvalidOperationException("Invalid folder path"));

                file.SaveAs(fullPath);
                return $"{folderPath}/{fileName}".Replace("~", string.Empty);
            }
            catch
            {
                return null;
            }
        }

        public string SetDefaultPhotoPath(string defaultPath) =>
            string.IsNullOrEmpty(defaultPath) ? 
            throw new Exception("Default Photo Path Not Found") 
            : defaultPath;
    }
}
