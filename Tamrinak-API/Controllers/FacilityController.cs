using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FacilityDtos;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.Services.FacilityService;
using Tamrinak_API.Services.ImageService;

namespace Tamrinak_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FacilityController : ControllerBase
	{
		private readonly IFacilityService _facilityService;
		private readonly IImageService _imageService;
		public FacilityController(IFacilityService facilityService, IImageService imageService)
		{
			_facilityService = facilityService;
			_imageService = imageService;
		}
		//[Authorize(Roles = "Admin")]//TODO
		[HttpPost("add-facility")]
		public async Task<IActionResult> AddFacility(AddFacilityDto dto)
		{
			try
			{
				var facility = await _facilityService.AddFacilityAsync(dto);
				return Ok(facility);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("get-facility")]
		public async Task<IActionResult> GetFacility(int id)
		{
			var facility = await _facilityService.GetFacilityDetailsAsync(id);
			return Ok(facility);
		}

		[HttpGet("get-facilities")]
		public async Task<IActionResult> Getfacilities()
		{
			var facilities = await _facilityService.GetFacilitiesAsync();
			return Ok(facilities);
		}

		[HttpPut("update-facility")]
		public async Task<IActionResult> UpdateFacility(int id, UpdateFacilityDto dto)
		{
			var newFacility = await _facilityService.UpdateFacilityDtoAsync(id, dto);// _facilityService.GetFacilityAsync(id);
			return Ok(newFacility);
		}

		[HttpPost("add-facility-image")]
		public async Task<IActionResult> AddFacilityImage(int facilityId, IFormFile formFile)
		{
			var facility = await _facilityService.GetFacilityAsync(facilityId);
			var canAdd = await _imageService.CanAddEntityImagesAsync<Facility>(facilityId, 15);
			if (canAdd)
			{
				var url = await _imageService.UploadImageAsync(formFile, "facilities");
				var image = new Image
				{
					FacilityId = facilityId,
					Url = url
				};
				await _imageService.AddImageAsync(image);
				await _imageService.UpdateImageAsync(image);
				await _facilityService.UpdateFacilityAsync(facility);
				return Ok();
			}
			else
			{
				return BadRequest("Max number of images for this facility");
			}

		}

		[HttpDelete("remove-facility")]
		public async Task<IActionResult> DeleteFacility(int facilityId)
		{
			var facility = await _facilityService.GetFacilityWithImagesAsync(facilityId);
			if (facility == null)
				return NotFound();
			var facilityImages = facility.Images.ToList();
			foreach (var image in facilityImages)
			{
				await _imageService.DeleteImageAsync(image.Url);
			}
			var result = await _facilityService.DeleteFacilityAsync(facilityId);
			if (!result)
				return NotFound("Facility not found");

			return Ok("Facility deleted");
		}

		[HttpDelete("delete-facility-image")]
		public async Task<IActionResult> DeleteFacilityImage(int facilityId, int imageId)
		{
			var image = await _imageService.GetImageAsync(imageId);
			if (image == null || image.FacilityId != facilityId)
				return NotFound("Image not found for this Facility.");

			await _imageService.DeleteImageAsync(image.Url);

			return Ok("Image deleted successfully.");

		}

		[HttpGet("get-facility-photo")]
		public async Task<IActionResult> GetFacilityPhoto(int id)
		{
			var image = await _imageService.GetImageAsync(id);
			if (image == null)
				return NotFound("Image not found.");

			var imageFileName = Path.GetFileName(image.Url);


			var publicUrl = $"{Request.Scheme}://{Request.Host}/uploads/fields/{imageFileName}";

			return Ok(publicUrl);
		}

		[HttpGet("get-ren-facility-photo")]
		public async Task<IActionResult> GetRenFacilityPhoto(int id)
		{
			var image = await _imageService.GetImageAsync(id);
			if (image == null)
				return NotFound("Image not found.");

			var imageFileName = Path.GetFileName(image.Url);
			var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "facilities", imageFileName);

			var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			var contentType = _imageService.GetContentType(imagePath);
			return File(stream, contentType);

		}

		[HttpGet("get-facility-photo-list")]
		public async Task<IActionResult> GetFacilityPhotoList(int facilityId)
		{
			var images = await _imageService.GetImagesAsync(facilityId, "facility");
			if (images == null)
				return NotFound("Images not found.");

			var result = images.Select(img => new
			{
				img.Id,
				FilePath = $"{Request.Scheme}://{Request.Host}/uploads/facilities/{Path.GetFileName(img.Url)}"
			});

			return Ok(result);
		}

		[HttpGet("by-sport/{sportId}")]
		public async Task<IActionResult> GetFacilitiesBySport(int sportId)
		{
			var result = await _facilityService.GetFacilitiesBySportAsync(sportId);
			if (!result.Any())
				return NotFound("No facilities found for this sport.");
			return Ok(result);
		}

	}
}
