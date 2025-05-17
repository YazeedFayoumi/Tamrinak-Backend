using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.DTO.PaginationDto;
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

		//[Authorize(Roles = "Admin")] // TODO
		[HttpPost("field")]
		public async Task<IActionResult> AddField(AddFieldDto dto)
		{
			try
			{
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var field = await _fieldService.AddFieldAsync(dto, userId);
				return Ok(field);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("field/{id}")]
		public async Task<IActionResult> GetField(int id)
		{
			var field = await _fieldService.GetFieldDetailsAsync(id);
			return Ok(field);
		}

		[HttpGet("all-fields")]
		public async Task<IActionResult> GetFields()
		{
			var fields = await _fieldService.GetFieldsAsync();
			return Ok(fields);
		}

		[HttpPut("field")]
		public async Task<IActionResult> UpdateField(int id, UpdateFieldDto dto)
		{
			await _fieldService.UpdateFieldDtoAsync(id, dto);
			var newField = await _fieldService.GetFieldAsync(id);
			return Ok(newField);
		}

		[HttpPost("field-images")]
		public async Task<IActionResult> AddFieldImages(int fieldId, List<IFormFile> formFiles)
		{
			try
			{
				var field = await _fieldService.GetFieldAsync(fieldId);
				if (field == null)
					return NotFound("Field not found.");

				if (formFiles == null || !formFiles.Any())
					return BadRequest("No files uploaded.");

				int existingCount = (await _imageService.GetImagesAsync(fieldId, "field")).Count();
				int maxImages = 10;

				if (existingCount >= maxImages)
					return BadRequest("Max number of images for this field.");

				int allowed = maxImages - existingCount;
				var filesToUpload = formFiles.Take(allowed);

				foreach (var file in filesToUpload)
				{
					// Upload image as Base64 and save in the database
					var base64Image = await _imageService.UploadImageAsync(file, "fields");
					var image = new Image
					{
						FieldId = fieldId,
						Base64Data = base64Image // Save Base64 image data
					};
					await _imageService.AddImageAsync(image);
				}

				await _fieldService.UpdateFieldAsync(field);
				return Ok("Images uploaded successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error uploading images: " + ex.Message);
				return StatusCode(500, "Internal Server Error: " + ex.Message);
			}
		}

        



        [HttpDelete("field")]
		public async Task<IActionResult> DeleteField(int fieldId)
		{
			var field = await _fieldService.GetFieldWithImagesAsync(fieldId);
			if (field == null)
				return NotFound();

			// Delete images based on Base64 data
			var fieldImages = field.Images.ToList();
			foreach (var image in fieldImages)
			{
				await _imageService.DeleteImageAsync(image.Base64Data); // Use Base64Data for deletion
			}

			var result = await _fieldService.DeleteFieldAsync(fieldId);
			if (!result)
				return NotFound("Field not found");

			return Ok("Field deleted");
		}

		[HttpDelete("field-image")]
		public async Task<IActionResult> DeleteFieldImage(int fieldId, int imageId)
		{
			var image = await _imageService.GetImageAsync(imageId);
			if (image == null || image.FieldId != fieldId)
				return NotFound("Image not found for this field.");

			await _imageService.DeleteImageAsync(image.Base64Data); // Use Base64Data for deletion
			return Ok("Image deleted successfully.");
		}

		[HttpGet("field-photo")]
		public async Task<IActionResult> GetFieldPhoto(int id)
		{
			var image = await _imageService.GetImageAsync(id);
			if (image == null)
				return NotFound("Image not found.");

			// Return Base64 image as data URI
			var base64Image = image.Base64Data;
			var dataUri = $"data:image/jpeg;base64,{base64Image}";

			return Ok(new { Image = dataUri });
		}

		[HttpGet("ren-field-photo")]
		public async Task<IActionResult> GetRenFieldPhoto(int id)
		{
			var image = await _imageService.GetImageAsync(id);
			if (image == null)
				return NotFound("Image not found.");

			// Return Base64 image as file content
			var base64Image = image.Base64Data;
			var byteArray = Convert.FromBase64String(base64Image);
			var contentType = _imageService.GetContentTypeFromBase64(base64Image);

			return File(byteArray, contentType);
		}

		[HttpGet("field-photo-list")]
		public async Task<IActionResult> GetFieldPhotoList(int fieldId)
		{
			var images = await _imageService.GetImagesAsync(fieldId, "field");
			if (images == null)
				return NotFound("Images not found.");

			// Return Base64 data for each image
			var result = images.Select(img => new
			{
				img.Id,
				ImageData = $"data:image/jpeg;base64,{img.Base64Data}"
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

		[HttpGet("sport-images/{sportId}")]
		public async Task<IActionResult> GetSportImages(int sportId)
		{
			var images = await _imageService.GetImagesAsync(sportId, "sport");

			if (!images.Any())
				return NotFound("No images found for this sport.");

			var imageData = images.Select(i => $"data:image/jpeg;base64,{i.Base64Data}").ToList();

			return Ok(imageData);
		}
        /*[HttpDelete("delete-field-images/{fieldId}")]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteFieldImages(int fieldId)
        {
            var images = await _imageService.GetImagesAsync(fieldId, "field");
            foreach (var img in images)
            {
                await _imageService.DeleteImageAsync();
            }
            return Ok("All images deleted.");
        }*/

        [HttpPut("{fieldId}/archive")]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> SoftDeleteField(int fieldId)
        {
            var result = await _fieldService.SetUnavailableFieldAsync(fieldId);
            if (!result)
                return NotFound("Field not found");

            return Ok("Field has been archived (soft deleted).");
        }

        [HttpPut("{fieldId}/reactivate")]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ReactivateField(int fieldId)
        {
            var result = await _fieldService.ReactivateFieldAsync(fieldId);
            if (!result)
                return NotFound("Field not found.");
            return Ok("Field reactivated.");
        }

        [HttpGet("pag-fields/by-sport/{sportId}")]
        public async Task<IActionResult> GetPagFieldsBySport(int sportId, [FromQuery] PaginationDto pagination)
        {
            var result = await _fieldService.GetPagFieldsBySportAsync(sportId, pagination);
            return Ok(result);
        }

    }
}
