using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.Services.ImageService
{
	public interface IImageService
	{
		Task<string> UploadImageAsync(IFormFile file, string folderName);
		Task AddImageAsync(Image image);
		Task<bool> CanAddEntityImagesAsync<TEntity>(int entityId, int maxImages) where TEntity : class;
		Task<bool> DeleteImageAsync(string imageUrl);
		Task<Image> GetImageAsync(int id);
		Task<IEnumerable<Image>> GetImagesAsync(int entityId, string entityType);
		public string GetContentType(string path);
		Task UpdateImageAsync(Image image);
	}
}
