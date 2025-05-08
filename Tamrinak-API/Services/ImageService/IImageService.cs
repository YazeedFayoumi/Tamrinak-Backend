using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.Services.ImageService
{
	public interface IImageService
	{
		Task<string> UploadImageAsync(IFormFile file, string folderName);
		Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folderName);
        Task AddImageAsync(Image image);
		Task<bool> CanAddEntityImagesAsync<TEntity>(int entityId, int maxImages) where TEntity : class;
		Task<bool> DeleteImageAsync(string base64Data);
		Task<Image> GetImageAsync(int id);
		Task<IEnumerable<Image>> GetImagesAsync(int entityId, string entityType);
		string GetContentType(string path);
		Task UpdateImageAsync(Image image);
		string GetContentTypeFromBase64(string base64Data);
	}
}
