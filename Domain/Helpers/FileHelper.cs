using System.IO;
using System.Web;

namespace Domain.Helpers
{
    public static class FileHelper
    {
        public static string UploadPhoto(HttpPostedFileBase file, string folder, string fileName)
        {
            if (file == null || string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(fileName))
                return null;

            try
            {
                string folderPath = HttpContext.Current.Server.MapPath(folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                file.SaveAs(filePath);

                return $"{folder}/{fileName}";
            }
            catch
            {
                return null;
            }
        }
    }
}