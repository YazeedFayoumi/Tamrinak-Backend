using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.SportDtos;

namespace Tamrinak_API.Services.SportService
{
    public interface ISportService
    {
        /*   Task<List<SportDto>> GetAllAsync();
           Task<SportDto> GetByIdAsync(int id);
           Task<SportDto> CreateAsync(AddSportDto dto);
           Task<bool> UpdateAsync(int id, UpdateSportDto dto);
           Task<bool> DeleteAsync(int id);*/
        Task<SportDto> AddSportAsync(AddSportDto dto);
        Task<IEnumerable<SportDto>> GetAllSportsAsync();
        Task<Sport> GetSportByIdAsync(int id);
        Task UpdateSportAsync(int id, SportDto dto);
        Task DeleteSportAsync(int id);
    }
}
