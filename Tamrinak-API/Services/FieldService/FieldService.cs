using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X9;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.DTO.UserAuthDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.FieldService
{
    public class FieldService : IFieldService
    {
        private readonly IGenericRepo<Field> _fieldRepo;
        private readonly IGenericRepo<Image> _imageRepo;
        public FieldService(IGenericRepo<Field> fieldRepo, IGenericRepo<Image> imageRepo)
        {
            _fieldRepo = fieldRepo;
            _imageRepo = imageRepo;
        }

        public async Task<Field> AddFieldAsync(AddFieldDto dto)
        {
            var field = new Field
            {
                Name = dto.Name,
                LocationDesc = dto.LocationDesc,
                SportId = dto.SportId,
                PhoneNumber = dto.PhoneNumber,
                LocationMap = dto.LocationMap,
                IsIndoor = dto.IsIndoor,
                HasLighting = dto.HasLighting,
                OpenTime = dto.OpenTime,
                CloseTime = dto.CloseTime,
                Capacity = dto.Capacity,
                PricePerHour = dto.PricePerHour
            };

            var createdField = await _fieldRepo.CreateAsync(field);
            await _fieldRepo.SaveAsync();
            return createdField;
        }

        public async Task<bool> DeleteFieldAsync(int id)
        {
            var field = await _fieldRepo.GetByConditionAsync(f => f.FieldId == id, include: f => f.Images);

            if (field == null)
                return false;

            var imagesToDelete = field.Images.ToList(); 

            foreach (var image in imagesToDelete)
            {
                await _imageRepo.DeleteAsync(image); 
            }

            await _fieldRepo.DeleteAsync(field);
            await _fieldRepo.SaveAsync();
            return true;
        }

        public async Task<Field> GetFieldAsync(int id)
        {
            return await _fieldRepo.GetAsync(id) ?? throw new KeyNotFoundException("Field not found");
        }

        public async Task<IEnumerable<FieldDto>> GetFieldsAsync()
        {
            var fields = await _fieldRepo.GetAllAsync();

            return fields.Select(f => new FieldDto
            {
                Id = f.FieldId,
                Name = f.Name,
                LocationDesc = f.LocationDesc,
                PhoneNumber = f.PhoneNumber,
                OpenTime = f.OpenTime,
                CloseTime = f.CloseTime,
                PricePerHour = f.PricePerHour

            }).ToList();
        }

        public async Task UpdateFieldDtoAsync(int id, FieldDto dto)
        {
            var field = await _fieldRepo.GetAsync(id);
            if (field == null)
                throw new KeyNotFoundException("Field not found");

            field.Name = dto.Name;
            field.LocationDesc = dto.LocationDesc;
            field.LocationMap = dto.LocationMap;
            field.PricePerHour = dto.PricePerHour;

            await _fieldRepo.UpdateAsync(field);
            await _fieldRepo.SaveAsync();    
        }

        public async Task UpdateFieldAsync(Field field)
        {
            var oldField = await _fieldRepo.GetAsync(field.FieldId);
         
            await _fieldRepo.UpdateAsync(oldField);
            await _fieldRepo.SaveAsync();
        }

        public async Task<Field> GetFieldWithImagesAsync(int fieldId)
        {
            return await _fieldRepo.GetByConditionIncludeAsync(
               f => f.FieldId == fieldId,
               q => q.Include(f => f.Images)
           );
        }
    }
}

