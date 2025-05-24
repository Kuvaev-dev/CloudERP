namespace Domain.UtilsAccess
{
    public interface IImageUploadHelper
    {
        Task<string?> UploadImageAsync(object file, string folderName);
    }
}
