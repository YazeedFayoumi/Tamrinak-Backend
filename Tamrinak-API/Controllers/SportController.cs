using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.DTO.SportDtos;
using Tamrinak_API.Services.SportService;

namespace Tamrinak_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportController : ControllerBase 
    {
        private readonly ISportService _sportService;
        public SportController(ISportService sportService)
        { 
            _sportService = sportService;
        }

		//[Authorize(Roles = "Admin")]//TODO
		[HttpPost("add-sport")]
        public async Task<IActionResult> AddSport(AddSportDto dto, IFormFile formFile)
        {
            try
            {
                var sport = await _sportService.AddSportAsync(dto);
                return Ok(sport);
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
            var sport = await _sportService.GetSportByIdAsync(id);
            return Ok(sport);
        }
        [HttpPut("update-sport")]
        public async Task<IActionResult> UpdateSport(int id, SportDto dto)
        {
            await _sportService.UpdateSportAsync(id, dto);
            return Ok(dto);
        }
        [HttpDelete("remove-sport")]
        public async Task<IActionResult> DeleteSport(int id)
        {
            await _sportService.DeleteSportAsync(id);
            return Ok();
        }

    }
}
