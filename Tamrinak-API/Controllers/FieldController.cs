using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.Services.FieldService;
using Tamrinak_API.Services.ImageService;

namespace Tamrinak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;
        private readonly IImageService _imageService;
        public FieldController(IFieldService fieldService, IImageService imageService)
        {
            _fieldService = fieldService;
            _imageService = imageService;
        }

		//[Authorize(Roles = "Admin")]//TODO
		[HttpPost("add-field")]
        public async Task<IActionResult> AddField(AddFieldDto dto)
        {
            try
            {
                var field = await _fieldService.AddFieldAsync(dto);
                return Ok(field);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-field")]
        public async Task<IActionResult> GetField(int id)
        {
            var field = await _fieldService.GetFieldDetailsAsync(id);
            return Ok(field);
        }

        [HttpGet("get-fields")]
        public async Task<IActionResult> GetFields()
        {
            var fields = await _fieldService.GetFieldsAsync();
            return Ok(fields);
        }

        [HttpPut("update-field")]
        public async Task<IActionResult> UpdateField(int id, UpdateFieldDto dto)
        {
            await _fieldService.UpdateFieldDtoAsync(id, dto);
            var newField = _fieldService.GetFieldAsync(id);
            return Ok(newField);
        }

        [HttpPost("add-field-image")]
        public async Task<IActionResult> AddFieldImage(int fieldId, IFormFile formFile)
        {
            var field = await _fieldService.GetFieldAsync(fieldId);
            var canAdd = await _imageService.CanAddEntityImagesAsync<Field>(fieldId, 10);
            if (canAdd)
            {
                var url = await _imageService.UploadImageAsync(formFile, "fields");
                var image = new Image
                {
                    FieldId = fieldId,
                    Url = url
                };
                await _imageService.AddImageAsync(image);
                await _imageService.UpdateImageAsync(image);
                await _fieldService.UpdateFieldAsync(field);
                return Ok();
            }
            else
            {
                return BadRequest("Max number of images for this field");
            }
            
          
        }

        [HttpDelete("remove-field")]
        public async Task<IActionResult> DeleteField(int fieldId)
        {
            var field = await _fieldService.GetFieldWithImagesAsync(fieldId);
            if (field == null)
                return NotFound();
            var fieldImages = field.Images.ToList();
            foreach (var image in fieldImages)
            {
                await _imageService.DeleteImageAsync(image.Url); 
            }
            var result = await _fieldService.DeleteFieldAsync(fieldId);
            if (!result)
                return NotFound("Field not found");

            return Ok("Field deleted");
        }

        [HttpDelete("delete-field-image")]
        public async Task<IActionResult> DeleteFieldImage(int fieldId, int imageId)
        {
            var image = await _imageService.GetImageAsync(imageId);
            if (image == null || image.FieldId != fieldId)
                return NotFound("Image not found for this field.");

            await _imageService.DeleteImageAsync(image.Url);

            return Ok("Image deleted successfully.");

        }

        [HttpGet("get-field-photo")]
        public async Task<IActionResult> GetFieldPhoto(int id)
        {
            var image = await _imageService.GetImageAsync(id);
            if (image == null)
                return NotFound("Image not found.");

            var imageFileName = Path.GetFileName(image.Url);

           
            var publicUrl = $"{Request.Scheme}://{Request.Host}/uploads/fields/{imageFileName}";

            return Ok(publicUrl);
        }

        [HttpGet("get-ren-field-photo")]
        public async Task<IActionResult> GetRenFieldPhoto(int id)
        {
            var image = await _imageService.GetImageAsync(id);
            if (image == null)
                return NotFound("Image not found.");

            var imageFileName = Path.GetFileName(image.Url);
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "fields", imageFileName);

            var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = _imageService.GetContentType(imagePath);
            return File(stream, contentType);

        }

        [HttpGet("get-field-photo-list")]
        public async Task<IActionResult> GetFieldPhotoList(int fieldId)
        {
            var images = await _imageService.GetImagesAsync(fieldId, "field");
            if (images == null)
                return NotFound("Images not found.");

            var result = images.Select(img => new
            {
                img.Id,
                FilePath = $"{Request.Scheme}://{Request.Host}/uploads/fields/{Path.GetFileName(img.Url)}"
            });

            return Ok(result);
        }

        [HttpGet("by-sport/{sportId}")]
        public async Task<IActionResult> GetFieldsBySport(int sportId)
        {
            var result = await _fieldService.GetFieldsBySportAsync(sportId);
            if (!result.Any())
                return NotFound("No fields found for this sport.");
            return Ok(result);
        }

        [HttpGet("get-sport-images/{sportId}")]
        public async Task<IActionResult> GetSportImages(int sportId)
        {
            var images = await _imageService.GetImagesAsync(sportId, "sport");

            if (!images.Any())
                return NotFound("No images found for this sport.");

            var imageUrls = images.Select(i => i.Url).ToList();

            return Ok(imageUrls);
        }


    }
}

