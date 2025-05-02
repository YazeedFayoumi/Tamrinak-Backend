using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FieldDtos;

namespace Tamrinak_API.Services.FieldService
{
    public interface IFieldService
    {
        Task<Field> AddFieldAsync(AddFieldDto dto);
        Task<Field> GetFieldAsync(int id);
        Task<IEnumerable<FieldDto>> GetFieldsAsync();
        Task UpdateFieldDtoAsync(int id, FieldDto dto);
        Task UpdateFieldAsync(Field field);
        Task<bool> DeleteFieldAsync(int id);
        Task<Field>  GetFieldWithImagesAsync(int fieldId);   
    }
}
