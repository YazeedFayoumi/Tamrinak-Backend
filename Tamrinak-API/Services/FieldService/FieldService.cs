using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X9;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.DTO.SportDtos;
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

		public async Task<FieldDto> AddFieldAsync(AddFieldDto dto)
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
				OpenTime = TimeOnly.Parse(dto.OpenTime),
				CloseTime = TimeOnly.Parse(dto.CloseTime),
				Capacity = dto.Capacity,
				PricePerHour = dto.PricePerHour
			};

			var createdField = await _fieldRepo.CreateAsync(field);
			await _fieldRepo.SaveAsync();
			return new FieldDto
			{
				Id = createdField.FieldId,
				Name = createdField.Name,
				LocationDesc = createdField.LocationDesc,
				LocationMap = createdField.LocationMap,
				IsIndoor = createdField.IsIndoor,
				HasLighting = createdField.HasLighting,
				OpenTime = createdField.OpenTime.ToString("HH:mm"),
				CloseTime = createdField.CloseTime.ToString("HH:mm"),
				Capacity = createdField.Capacity,
				PhoneNumber = createdField.PhoneNumber,
				PricePerHour = createdField.PricePerHour,
				SportId = createdField.SportId,
			};
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
		public async Task<FieldDetailsDto> GetFieldDetailsAsync(int id)
		{
			var field = await _fieldRepo.GetByConditionIncludeAsync(
				f => f.FieldId == id,
				include: q => q.Include(f => f.Sport)
			);

			if (field == null)
				throw new KeyNotFoundException("Field not found");

			return new FieldDetailsDto
			{
				FieldId = field.FieldId,
				Name = field.Name,
				LocationDesc = field.LocationDesc,
				LocationMap = field.LocationMap,
				PhoneNumber = field.PhoneNumber,
				Capacity = field.Capacity,
				PricePerHour = (decimal)field.PricePerHour,
				HasLighting = field.HasLighting,
				IsIndoor = field.IsIndoor,
				OpenTime = field.OpenTime.ToString("HH:mm"),
				CloseTime = field.CloseTime.ToString("HH:mm"),
				Sport = new SportBasicDto
				{
					Id = field.Sport.SportId,
					Name = field.Sport.Name
				}
			};
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
				OpenTime = f.OpenTime.ToString("HH:mm"),
				CloseTime = f.CloseTime.ToString("HH:mm"),
				PricePerHour = f.PricePerHour

			}).ToList();
		}

		public async Task UpdateFieldDtoAsync(int id, UpdateFieldDto dto)
		{
			var field = await _fieldRepo.GetAsync(id);
			if (field == null)
				throw new KeyNotFoundException("Field not found");

			field.Name = dto.Name;
			field.LocationDesc = dto.LocationDesc;
			field.LocationMap = dto.LocationMap;
			field.PricePerHour = dto.PricePerHour;
			field.OpenTime = TimeOnly.Parse(dto.OpenTime);
			field.CloseTime = TimeOnly.Parse(dto.CloseTime);
			field.PhoneNumber = dto.PhoneNumber;
			field.Capacity = dto.Capacity;
			field.HasLighting = dto.HasLighting;
			field.IsIndoor = dto.IsIndoor;

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

		public async Task<List<FieldBySportDto>> GetFieldsBySportAsync(int sportId)
		{
			var fields = await _fieldRepo.GetListByConditionIncludeAsync(
				f => f.SportId == sportId,
				include: q => q.Include(f => f.Sport).Include(f => f.Images)
			);

			var result = fields.Select(f => new FieldBySportDto
			{
				Id = f.FieldId,
				Name = f.Name,
				LocationDesc = f.LocationDesc,
				PricePerHour = f.PricePerHour,
				Sport = new SportBasicDto
				{
					Id = f.Sport.SportId,
					Name = f.Sport.Name
				},
				// Modify this part to use Base64 instead of Url
				Images = f.Images.Select(img => img.Base64Data).ToList() // Changed from img.Url to img.Base64Data
			}).ToList();

			return result;
		}

	}
}

