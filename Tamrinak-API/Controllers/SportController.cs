using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.SportDtos;
using Tamrinak_API.Services.ImageService;
using Tamrinak_API.Services.SportService;

namespace Tamrinak_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SportController : ControllerBase
	{
		private readonly ISportService _sportService;
		private readonly IImageService _imageService;
		public SportController(ISportService sportService, IImageService imageService)
		{
			_sportService = sportService;
			_imageService = imageService;
		}

		//[Authorize(Roles = "Admin")] // TODO
		[HttpPost("sport")]
		public async Task<IActionResult> AddSport([FromForm] AddSportDto dto, IFormFile formFile)
		{
			try
			{
				// Upload image first
				var base64Image = await _imageService.UploadImageAsync(formFile, "sports");

				// Then add sport
				var sportDto = await _sportService.AddSportAsync(dto);

				var image = new Image
				{
					SportId = sportDto.Id,
					Base64Data = base64Image
				};

				await _imageService.AddImageAsync(image);

				var sport = await _sportService.GetSportByIdAsync(sportDto.Id);
				await _sportService.UpdateSportAsync(sport);

				return Ok(sportDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("all-sports")]
		public async Task<IActionResult> GetAllSports()
		{
			try
			{
				var sports = await _sportService.GetAllSportsAsync();
				return Ok(sports);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("sport/{id}")]
		public async Task<IActionResult> GetSport(int id)
		{
			try
			{
				var sport = await _sportService.GetSportDetailAsync(id);
				if (sport == null)
				{
					return NotFound($"Sport with ID {id} not found");
				}
				return Ok(sport);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		[HttpPut("sport")]
		public async Task<IActionResult> UpdateSport(int id, UpdateSportDto dto)
		{
			try
			{
				await _sportService.UpdateSportDtoAsync(id, dto);
				return Ok(dto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("sport")]
		public async Task<IActionResult> DeleteSport(int id)
		{
			try
			{
				await _sportService.DeleteSportAsync(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("sport-image/{id}")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UpdateSportImage(int id, IFormFile formFile)
		{
			try
			{
				if (formFile == null || formFile.Length == 0)
					return BadRequest("No file uploaded");

				var sport = await _sportService.GetSportByIdAsync(id);
				if (sport == null)
					return NotFound("Sport not found");

				// Get existing images for the sport
				var existingImages = await _imageService.GetImagesAsync(id, "sport");

				// Delete existing images from the database
				foreach (var img in existingImages.ToList())
				{
					await _imageService.DeleteImageAsync(img.Base64Data); // Use Base64Data for deletion
				}

				// Upload new image as Base64
				var base64Image = await _imageService.UploadImageAsync(formFile, "sports");

				// Create a new image entity
				var image = new Image
				{
					SportId = id,
					Base64Data = base64Image // Save the Base64 image data
				};

				await _imageService.AddImageAsync(image);

				return Ok("Image updated successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
