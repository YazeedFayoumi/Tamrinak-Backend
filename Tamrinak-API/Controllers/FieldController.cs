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

		[Authorize(Roles = "Admin, SuperAdmin, VenueManager")]
		[HttpPost("field")]
		public async Task<IActionResult> AddField(AddFieldDto dto)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

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
			try
			{
				var field = await _fieldService.GetFieldDetailsAsync(id);
				return Ok(field);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("all-fields")]
		public async Task<IActionResult> GetFields()
		{
			try
			{
				var fields = await _fieldService.GetFieldsAsync();
				return Ok(fields);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("field")]
		public async Task<IActionResult> UpdateField(int id, UpdateFieldDto dto)
		{
			try
			{
				await _fieldService.UpdateFieldDtoAsync(id, dto);
				var newField = await _fieldService.GetFieldAsync(id);
				return Ok(newField);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("field-images")]
		public async Task<IActionResult> AddFieldImages([FromForm] int fieldId, List<IFormFile> formFiles)
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
					var base64Image = await _imageService.UploadImageAsync(file, "fields");
					var image = new Image
					{
						FieldId = fieldId,
						Base64Data = base64Image 
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
			try
			{
				var field = await _fieldService.GetFieldWithImagesAsync(fieldId);
				if (field == null)
					return NotFound();

				var fieldImages = field.Images.ToList();
				foreach (var image in fieldImages)
				{
					await _imageService.DeleteImageAsync(image.Base64Data);
				}

				var result = await _fieldService.DeleteFieldAsync(fieldId);
				if (!result)
					return NotFound("Field not found");

				return Ok("Field deleted");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("field-image")]
		public async Task<IActionResult> DeleteFieldImage(int fieldId, int imageId)
		{
			try
			{
				var image = await _imageService.GetImageAsync(imageId);
				if (image == null || image.FieldId != fieldId)
					return NotFound("Image not found for this field.");

				await _imageService.DeleteImageAsync(image.Base64Data); 
				return Ok("Image deleted successfully.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("field-photo")]
		public async Task<IActionResult> GetFieldPhoto(int id)
		{
			try
			{
				var image = await _imageService.GetImageAsync(id);
				if (image == null)
					return NotFound("Image not found.");

				var base64Image = image.Base64Data;
				var dataUri = $"data:image/jpeg;base64,{base64Image}";

				return Ok(new { Image = dataUri });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("ren-field-photo")]
		public async Task<IActionResult> GetRenFieldPhoto(int id)
		{
			try
			{
				var image = await _imageService.GetImageAsync(id);
				if (image == null)
					return NotFound("Image not found.");


				var base64Image = image.Base64Data;
				var byteArray = Convert.FromBase64String(base64Image);
				var contentType = _imageService.GetContentTypeFromBase64(base64Image);

				return File(byteArray, contentType);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("field-photo-list")]
		public async Task<IActionResult> GetFieldPhotoList(int fieldId)
		{
			try
			{
				var images = await _imageService.GetImagesAsync(fieldId, "field");
				if (images == null)
					return NotFound("Images not found.");

				var result = images.Select(img => new
				{
					img.Id,
					ImageData = $"data:image/jpeg;base64,{img.Base64Data}"
				});

				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("by-sport/{sportId}")]
		public async Task<IActionResult> GetFieldsBySport(int sportId)
		{
			try
			{
				var result = await _fieldService.GetFieldsBySportAsync(sportId);
				if (!result.Any())
					return NotFound("No fields found for this sport.");
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("sport-images/{sportId}")]
		public async Task<IActionResult> GetSportImages(int sportId)
		{
			try
			{
				var images = await _imageService.GetImagesAsync(sportId, "sport");

				if (!images.Any())
					return NotFound("No images found for this sport.");

				var imageData = images.Select(i => $"data:image/jpeg;base64,{i.Base64Data}").ToList();

				return Ok(imageData);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{fieldId}/archive")]
		//[Authorize(Roles = "Admin,SuperAdmin")]
		public async Task<IActionResult> SoftDeleteField(int fieldId)
		{
			try
			{
				var result = await _fieldService.SetUnavailableFieldAsync(fieldId);
				if (!result)
					return NotFound("Field not found");

				return Ok("Field has been archived (soft deleted).");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{fieldId}/reactivate")]
		//[Authorize(Roles = "Admin,SuperAdmin")]
		public async Task<IActionResult> ReactivateField(int fieldId)
		{
			try
			{
				var result = await _fieldService.ReactivateFieldAsync(fieldId);
				if (!result)
					return NotFound("Field not found.");
				return Ok("Field reactivated.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("pag-fields/by-sport/{sportId}")]
		public async Task<IActionResult> GetPagFieldsBySport(int sportId, [FromQuery] PaginationDto pagination)
		{
			try
			{
				var result = await _fieldService.GetPagFieldsBySportAsync(sportId, pagination);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}
