
using System.Linq.Expressions;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.Repository.GenericRepo;

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
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/{folderName}/{fileName}";
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
    }
}
