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

		//[Authorize(Roles = "Admin")]//TODO
		[HttpPost("add-sport")]
		public async Task<IActionResult> AddSport([FromForm] AddSportDto dto, IFormFile formFile)
		{
			try
			{
				var sportDto = await _sportService.AddSportAsync(dto);
				var sport = await _sportService.GetSportByIdAsync(sportDto.Id);
				var url = await _imageService.UploadImageAsync(formFile, "sports");
				var image = new Image
				{
					SportId = sportDto.Id,
					Url = url
				};
				await _imageService.AddImageAsync(image);
				await _imageService.UpdateImageAsync(image);
				await _sportService.UpdateSportAsync(sport);

				return Ok(sportDto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet("get-all-sports")]
		public async Task<IActionResult> GetAllSports()
		{
			var sports = await _sportService.GetAllSportsAsync();

			return Ok(sports);
		}
		[HttpGet("get-sport")]
		public async Task<IActionResult> GetSport(int id)
		{
			var sport = await _sportService.GetSportDetailAsync(id);
			return Ok(sport);
		}
		[HttpPut("update-sport")]
		public async Task<IActionResult> UpdateSport(int id, UpdateSportDto dto)
		{
			await _sportService.UpdateSportDtoAsync(id, dto);
			return Ok(dto);
		}
		[HttpDelete("remove-sport")]
		public async Task<IActionResult> DeleteSport(int id)
		{
			await _sportService.DeleteSportAsync(id);
			return Ok();
		}

		[HttpPost("update-sport-image/{id}")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UpdateSportImage(int id, IFormFile formFile)
		{
			if (formFile == null || formFile.Length == 0)
				return BadRequest("No file uploaded");

			var sport = await _sportService.GetSportByIdAsync(id);
			if (sport == null)
				return NotFound("Sport not found");

			// Get existing images
			var existingImages = await _imageService.GetImagesAsync(id, "sport");

			// Delete each one
			foreach (var img in existingImages.ToList())
			{
				await _imageService.DeleteImageAsync(img.Url);
			}

			// Upload new one
			var url = await _imageService.UploadImageAsync(formFile, "sports");

			var image = new Image
			{
				SportId = id,
				Url = url
			};

			await _imageService.AddImageAsync(image);

			return Ok("Image updated successfully");
		}

	}
}
