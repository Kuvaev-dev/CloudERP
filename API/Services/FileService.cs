using Domain.ServiceAccess;
using Utils.Interfaces;

namespace API.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task<string?> UploadPhotoAsync(IFile file, string folderPath, string fileName)
        {
            if (file?.ContentLength <= 0) return null;

            var fullFolderPath = Path.Combine(_env.WebRootPath, folderPath.TrimStart('~', '/'));
            var fullPath = Path.Combine(fullFolderPath, fileName);

            try
            {
                Directory.CreateDirectory(fullFolderPath);

                file.SaveAs(fullPath);
                return $"/{folderPath.TrimStart('~', '/')}/{fileName}";
            }
            catch
            {
                return null;
            }
        }

        public string SetDefaultPhotoPath(string defaultPath) =>
            string.IsNullOrEmpty(defaultPath)
                ? throw new Exception("Default Photo Path Not Found")
                : defaultPath;
    }
}
