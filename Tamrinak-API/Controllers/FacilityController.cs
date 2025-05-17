using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FacilityDtos;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.DTO.PaginationDto;
using Tamrinak_API.Services.FacilityService;
using Tamrinak_API.Services.ImageService;
using Tamrinak_API.Services.MembershipOfferService;

namespace Tamrinak_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FacilityController : ControllerBase
	{
		private readonly IFacilityService _facilityService;
		private readonly IImageService _imageService;
		// readonly IMembershipOfferService _membershipOfferService;

		public FacilityController(IFacilityService facilityService, IImageService imageService)
		{
			_facilityService = facilityService;
			_imageService = imageService;
		}

		//[Authorize(Roles = "Admin")] // TODO
		[HttpPost("facility")]
		public async Task<IActionResult> AddFacility(AddFacilityDto dto)
		{
			try
			{
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var facility = await _facilityService.AddFacilityAsync(dto, userId);
				return Ok(facility);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("facility/{id}")]
		public async Task<IActionResult> GetFacility(int id)
		{
			var facility = await _facilityService.GetFacilityDetailsAsync(id);
			return Ok(facility);
		}

		[HttpGet("facilities")]
		public async Task<IActionResult> Getfacilities()
		{
			var facilities = await _facilityService.GetFacilitiesAsync();
			return Ok(facilities);
		}

		[HttpPut("facility")]
		public async Task<IActionResult> UpdateFacility(int id, UpdateFacilityDto dto)
		{
			var newFacility = await _facilityService.UpdateFacilityDtoAsync(id, dto);// _facilityService.GetFacilityAsync(id);
			return Ok(newFacility);
		}

		[HttpPost("facility-image")]
		public async Task<IActionResult> AddFacilityImage(int facilityId, IFormFile formFile)
		{
			var facility = await _facilityService.GetFacilityAsync(facilityId);
			var canAdd = await _imageService.CanAddEntityImagesAsync<Facility>(facilityId, 15);
			if (canAdd)
			{
				// Upload the image as Base64
				var base64Image = await _imageService.UploadImageAsync(formFile, "facilities");

				var image = new Image
				{
					FacilityId = facilityId,
					Base64Data = base64Image // Store as Base64
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

        [HttpDelete("facility")]
		public async Task<IActionResult> DeleteFacility(int facilityId)
		{
			var facility = await _facilityService.GetFacilityWithImagesAsync(facilityId);
			if (facility == null)
				return NotFound();

			var facilityImages = facility.Images.ToList();
			foreach (var image in facilityImages)
			{
				await _imageService.DeleteImageAsync(image.Base64Data); // Use Base64Data for deletion
			}

			var result = await _facilityService.DeleteFacilityAsync(facilityId);
			if (!result)
				return NotFound("Facility not found");

			return Ok("Facility deleted");
		}

		[HttpDelete("facility-image")]
		public async Task<IActionResult> DeleteFacilityImage(int facilityId, int imageId)
		{
			var image = await _imageService.GetImageAsync(imageId);
			if (image == null || image.FacilityId != facilityId)
				return NotFound("Image not found for this Facility.");

			await _imageService.DeleteImageAsync(image.Base64Data); // Use Base64Data for deletion

			return Ok("Image deleted successfully.");
		}

		[HttpGet("facility-photo")]
		public async Task<IActionResult> GetFacilityPhoto(int id)
		{
			var image = await _imageService.GetImageAsync(id);
			if (image == null)
				return NotFound("Image not found.");

			// Return the Base64 data as the response
			return Ok(image.Base64Data);
		}

		[HttpGet("ren-facility-photo")]
		public async Task<IActionResult> GetRenFacilityPhoto(int id)
		{
			var image = await _imageService.GetImageAsync(id);
			if (image == null)
				return NotFound("Image not found.");

			var base64Image = image.Base64Data;

			// Return the Base64 data as a response
			return Ok(base64Image);
		}

		[HttpGet("facility-photo-list")]
		public async Task<IActionResult> GetFacilityPhotoList(int facilityId)
		{
			var images = await _imageService.GetImagesAsync(facilityId, "facility");
			if (images == null)
				return NotFound("Images not found.");

			var result = images.Select(img => new
			{
				img.Id,
				Base64Data = img.Base64Data // Return Base64 data
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

     /*   [HttpDelete("delete-facility-images/{facilityId}")]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteFieldImages(int fieldId)
        {
            var images = await _imageService.GetImagesAsync(fieldId, "facility");
            foreach (var img in images)
            {
                await _imageService.DeleteImageAsync(img.Url);
            }
            return Ok("All images deleted.");
        }
*/
        [HttpPut("{facilityId}/archive")]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> SetUnavailableFacility(int facilityId)
        {
            var success = await _facilityService.SetUnavailableFacilityAsync(facilityId);
            if (!success)
                return NotFound("Facility not found");

            return Ok("Facility has been archived (soft deleted).");
        }

        [HttpPut("{facilityId}/reactivate")]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ReactivateFacility(int facilityId)
        {
            var result = await _facilityService.ReactivateFacilityAsync(facilityId);
            if (!result)
                return NotFound("Facility not found.");
            return Ok("Facility reactivated.");
        }

        [HttpGet("pag-facilities/by-sport/{sportId}")]
        public async Task<IActionResult> GetPagFacilitiesBySport(int sportId, [FromQuery] PaginationDto pagination)
        {
            var result = await _facilityService.GetPagFacilitiesBySportAsync(sportId, pagination);
            return Ok(result);
        }

    }
}
