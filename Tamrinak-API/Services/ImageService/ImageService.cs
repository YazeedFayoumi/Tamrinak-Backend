using System.Linq.Expressions;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.Repository.GenericRepo;
using SystemImage = System.Drawing.Image;
using Image = Tamrinak_API.DataAccess.Models.Image;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Tamrinak_API.Services.ImageService
{
	public class ImageService : IImageService
	{
		private readonly IWebHostEnvironment _env;
		private readonly IGenericRepo<Image> _imageRepo;

		public ImageService(IWebHostEnvironment env, IGenericRepo<Image> imageRepo)
		{
			_env = env;
			_imageRepo = imageRepo;
		}
		public async Task<string> UploadImageAsync(IFormFile file, string folderName)
		{
			long maxSize = folderName == "user" ? 10 * 1024 * 1024 : 50 * 1024 * 1024;
			if (file.Length > maxSize)
				throw new InvalidOperationException("File size exceeds allowed limit.");

			using var image = SystemImage.FromStream(file.OpenReadStream());
			var width = image.Width;
			var height = image.Height;

			const int maxWidth = 2000;
			const int maxHeight = 2000;

			if (width > maxWidth || height > maxHeight)
				throw new InvalidOperationException($"Image resolution is too high. Maximum allowed is {maxWidth}x{maxHeight}.");

			if (folderName == "users")
			{
				if (width < 150 || height < 150)
					throw new InvalidOperationException("User profile image is too small. Minimum 150x150 required.");

				if (Math.Abs(width - height) > 10)
					throw new InvalidOperationException("User profile image must be square.");
			}
			else if (folderName == "facilities" || folderName == "fields")
			{
				if (width < 400 || height < 400)
					throw new InvalidOperationException("Image resolution too low for field/facility. Minimum 400x400 required.");
			}
			else
			{
				if (width < 300 || height < 300)
					throw new InvalidOperationException("Image resolution too low for sport. Minimum 300 required.");
			}

			using var ms = new MemoryStream();
			image.Save(ms, image.RawFormat);
			var imageBytes = ms.ToArray();
			var base64 = Convert.ToBase64String(imageBytes);

			return base64;
		}

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folderName)
        {
            var urls = new List<string>();

            foreach (var file in files)
            {
                var url = await UploadImageAsync(file, folderName); // Reuse your single-upload logic
                urls.Add(url);
            }

            return urls;
        }

        public async Task<bool> CanAddEntityImagesAsync<TEntity>(int entityId, int maxImages) where TEntity : class
		{
			Expression<Func<Image, bool>> condition = entity =>
				typeof(TEntity) == typeof(Facility) && entity.FacilityId == entityId ||
				typeof(TEntity) == typeof(Field) && entity.FieldId == entityId ||
				typeof(TEntity) == typeof(Sport) && entity.SportId == entityId;

			var images = await _imageRepo.GetListByConditionAsync(condition);
			return images.Count() < maxImages;
		}

		public async Task<bool> DeleteImageAsync(string base64Data)
		{
			var image = await _imageRepo.GetByConditionAsync(u => u.Base64Data == base64Data);

			if (image == null)
			{
                throw new Exception("Image not found");
            }
			else
			{
                try

                {
                    await _imageRepo.DeleteAsync(image);
                    await _imageRepo.SaveAsync();
                    return true;
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Delete failed: " + ex.Message);
                    return false;
                }
            }
            
		}

		public async Task UpdateImageAsync(Image image)
		{
			await _imageRepo.UpdateAsync(image);
			await _imageRepo.SaveAsync();
		}

		public async Task AddImageAsync(Image image)
		{
			await _imageRepo.AddAsync(image);
			await _imageRepo.SaveAsync();
		}

		public async Task<Image> GetImageAsync(int id)
		{
			var image = await _imageRepo.GetAsync(id);
			return image;
		}

		public string GetContentType(string path)
		{
			var ext = Path.GetExtension(path).ToLowerInvariant();
			return ext switch
			{
				".jpg" or ".jpeg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".bmp" => "image/bmp",
				".webp" => "image/webp",
				".svg" => "image/svg+xml",
				_ => "application/octet-stream",
			};
		}

		public async Task<IEnumerable<Image>> GetImagesAsync(int entityId, string entityType)
		{
			var images = await _imageRepo.GetListByConditionAsync(i =>
				(entityType == "field" && i.FieldId == entityId) ||
				(entityType == "sport" && i.SportId == entityId) ||
				(entityType == "facility" && i.FacilityId == entityId)
			);

			return images;
		}
		public string GetContentTypeFromBase64(string base64Data)
		{
			if (string.IsNullOrEmpty(base64Data))
				throw new ArgumentException("Base64 data is empty or null.", nameof(base64Data));
			const string prefix = "data:";
			if (!base64Data.StartsWith(prefix))
				throw new ArgumentException("Invalid Base64 data format. It should start with 'data:image/'", nameof(base64Data));
			int contentTypeEndIndex = base64Data.IndexOf(';');
			if (contentTypeEndIndex == -1)
				throw new ArgumentException("Invalid Base64 data format. Could not find the MIME type.", nameof(base64Data));

			string contentType = base64Data.Substring(prefix.Length, contentTypeEndIndex - prefix.Length);

			return contentType;
		}

	}
}
