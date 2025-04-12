namespace Tamrinak_API.Services.ImageService
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);
        Task<bool> CanAddEntityImagesAsync<TEntity>(int entityId, int maxImages) where TEntity : class;
    }
}
