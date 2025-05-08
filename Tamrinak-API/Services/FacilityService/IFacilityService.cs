using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FacilityDtos;
using Tamrinak_API.DTO.FieldDtos;

namespace Tamrinak_API.Services.FacilityService
{
	public interface IFacilityService
	{
		Task<FacilityDto> AddFacilityAsync(AddFacilityDto dto);
		Task<Facility> GetFacilityAsync(int id);
		Task<FacilityDetailsDto> GetFacilityDetailsAsync(int id);
		Task<IEnumerable<FacilityDto>> GetFacilitiesAsync();
		Task<FacilityDto> UpdateFacilityDtoAsync(int id, UpdateFacilityDto dto);
		Task UpdateFacilityAsync(Facility facility);
		Task<bool> DeleteFacilityAsync(int id);
		Task<Facility> GetFacilityWithImagesAsync(int facilityId);
		Task<List<FacilityBySportDto>> GetFacilitiesBySportAsync(int sportId);
	}
}
