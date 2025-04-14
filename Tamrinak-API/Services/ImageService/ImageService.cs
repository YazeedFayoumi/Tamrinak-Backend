
using System.Linq.Expressions;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.Repository.GenericRepo;
//using SixLabors.ImageSharp;
using SystemImage = System.Drawing.Image;
using Image = Tamrinak_API.DataAccess.Models.Image;

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
            long maxSize = folderName == "user" ? 1 * 1024 * 1024 : 5 * 1024 * 1024;
            if (file.Length > maxSize)
                throw new InvalidOperationException("File size exceeds allowed limit.");

            
            using var image = SystemImage.FromStream(file.OpenReadStream());
            var width = image.Width;
            var height = image.Height;

            if (folderName == "users")
            {
                
                if (width < 150 || height < 150)
                    throw new InvalidOperationException("User profile image is too small. Minimum 150x150 required.");

                if (Math.Abs(width - height) > 10) // allow slight variance (e.g., 302x300)
                    throw new InvalidOperationException("User profile image must be square.");
            }
            else if (folderName == "facility" || folderName == "field")
            {
                if (width < 800 || height < 600)
                    throw new InvalidOperationException("Image resolution too low for field/facility. Minimum 800x600 required.");
            }
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await file.CopyToAsync(stream);
            }
            var url = $"/uploads/{folderName}/{fileName}";
            await _imageRepo.SaveAsync();
            return url;

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
        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                var uriParts = imageUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (uriParts.Length < 3)
                    return false;

                var folder = uriParts[1]; // "users"
                var fileName = uriParts[2];
                var filePath = Path.Combine(_env.WebRootPath, "uploads", folder, fileName);


                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                await _imageRepo.SaveAsync();
                return true;
            }
            catch(Exception ex) 
            {
                Console.WriteLine("Delete failed: " + ex.Message);
                return false;
            }
            
        }
    }
}
