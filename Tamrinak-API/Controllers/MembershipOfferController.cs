using Microsoft.AspNetCore.Mvc;
using Tamrinak_API.DTO.MembershipOfferDtos;
using Tamrinak_API.Services.MembershipOfferService;

namespace Tamrinak_API.Controllers
{
	[Route("api/membership-offer")]
	[ApiController]
	public class MembershipOfferController : ControllerBase
	{
		private readonly IMembershipOfferService _service;

		public MembershipOfferController(IMembershipOfferService service)
		{
			_service = service;
		}

		[HttpPost("add-offer")]
		public async Task<IActionResult> AddOffer([FromBody] AddMembershipOfferDto dto)
		{
			try
			{
				var result = await _service.AddOfferAsync(dto);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("get/{facilityId}")]
		public async Task<IActionResult> GetOffers(int facilityId)
		{
			try
			{
				var offers = await _service.GetOffersForFacilityAsync(facilityId);
				return Ok(offers);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("remove/{offerId}")]
		public async Task<IActionResult> DeleteOffer(int offerId)
		{
			try
			{
				var deleted = await _service.DeleteOfferAsync(offerId);
				return deleted ? Ok() : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPut("update/{offerId}")]
		public async Task<IActionResult> UpdateOffer(int offerId, [FromBody] UpdateMembershipOfferDto dto)
		{
			try
			{
				var result = await _service.UpdateOfferAsync(offerId, dto);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}

}
