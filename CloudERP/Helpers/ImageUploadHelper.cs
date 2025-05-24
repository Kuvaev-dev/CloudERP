using Domain.UtilsAccess;

namespace CloudERP.Helpers
{
    public class ImageUploadHelper : IImageUploadHelper
    {
        public async Task<string?> UploadImageAsync(object file, string folderName)
        {
            var fileObj = file as IFormFile;
            if (file == null || fileObj.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileObj.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileObj.CopyToAsync(fileStream);
            }

            return $"/{folderName}/{uniqueFileName}";
        }
    }
}
