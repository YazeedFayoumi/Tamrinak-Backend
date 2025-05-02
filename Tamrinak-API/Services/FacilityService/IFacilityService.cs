using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FacilityDtos;
using Tamrinak_API.DTO.FieldDtos;

namespace Tamrinak_API.Services.FacilityService
{
    public interface IFacilityService
    {
        Task<Facility> AddFacilityAsync(AddFacilityDto dto);
        Task<Facility> GetFacilityAsync(int id);
        Task<IEnumerable<FacilityDto>> GetFacilitiesAsync();
        Task UpdateFacilityDtoAsync(int id, FacilityDto dto);
        Task UpdateFacilityAsync(Facility facility);
        Task<bool> DeleteFacilityAsync(int id);
        Task<Facility> GetFacilityWithImagesAsync(int facilityId);
    }
}
