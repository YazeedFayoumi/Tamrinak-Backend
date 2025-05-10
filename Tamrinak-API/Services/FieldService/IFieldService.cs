using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.DTO.PaginationDto;

namespace Tamrinak_API.Services.FieldService
{
	public interface IFieldService
	{
		Task<FieldDto> AddFieldAsync(AddFieldDto dto);
		Task<Field> GetFieldAsync(int id);
		Task<FieldDetailsDto> GetFieldDetailsAsync(int id);
		Task<IEnumerable<FieldDto>> GetFieldsAsync();
		Task UpdateFieldDtoAsync(int id, UpdateFieldDto dto);
		Task UpdateFieldAsync(Field field);
		Task<bool> DeleteFieldAsync(int id);
		Task<Field> GetFieldWithImagesAsync(int fieldId);
		Task<List<FieldBySportDto>> GetFieldsBySportAsync(int sportId);
        Task<List<FieldBySportDto>> GetPagFieldsBySportAsync(int sportId, PaginationDto dto);

        Task<bool> SetUnavailableFieldAsync(int fieldId);
		Task<bool> ReactivateFieldAsync(int fieldId);

    }
}
