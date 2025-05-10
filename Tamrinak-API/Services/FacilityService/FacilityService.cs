using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using Tamrinak_API.DataAccess.Configurartions;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FacilityDtos;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.DTO.PaginationDto;
using Tamrinak_API.DTO.SportDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.FacilityService
{
	public class FacilityService : IFacilityService
	{
		private readonly IGenericRepo<Facility> _facilityRepo;
		private readonly IGenericRepo<SportFacility> _sportFacilityRepo;
		private readonly IGenericRepo<Sport> _sportRepo;
		private readonly IGenericRepo<Image> _imageRepo;
		private readonly IGenericRepo<Membership> _membershipRepo;
		public FacilityService(IGenericRepo<Facility> facilityRepo, IGenericRepo<SportFacility> sportFacilityRepo, IGenericRepo<Membership> memRepo,
			IGenericRepo<Image> imageRepo,
			IGenericRepo<Sport> sportRepo)
		{
			_facilityRepo = facilityRepo;
			_sportFacilityRepo = sportFacilityRepo;
			_imageRepo = imageRepo;
			_sportRepo = sportRepo;
			_membershipRepo = memRepo;
		}

		public async Task<FacilityDto> AddFacilityAsync(AddFacilityDto dto)
		{
			var facility = new Facility
			{
				Name = dto.Name,
				LocationDesc = dto.LocationDesc,
				Type = dto.Type,
				PhoneNumber = dto.PhoneNumber,
				LocationMap = dto.LocationMap,
				PricePerMonth = dto.PricePerMonth,
				Description = dto.Description,
				OpenTime = TimeOnly.Parse(dto.OpenTime),
				CloseTime = TimeOnly.Parse(dto.CloseTime)
			};

			var createdFac = await _facilityRepo.CreateAsync(facility);
			await _facilityRepo.SaveAsync();

			foreach (var sportId in dto.SportIds)
			{
				var exists = await _sportRepo.ExistsAsync(s => s.SportId == sportId);
				if (!exists)
					throw new Exception($"Sport with ID {sportId} does not exist.");
			}

			foreach (var sportId in dto.SportIds)
			{
				var sportFacility = new SportFacility
				{
					FacilityId = facility.FacilityId,
					SportId = sportId
				};
				await _sportFacilityRepo.AddAsync(sportFacility);
			}

			return new FacilityDto
			{
				Id = facility.FacilityId,
				Name = facility.Name,
				LocationDesc = facility.LocationDesc,
				LocationMap = facility.LocationMap,
				PhoneNumber = facility.PhoneNumber,
				PricePerMonth = facility.PricePerMonth,
				IsAvailable = facility.IsAvailable,
				OpenTime = facility.OpenTime.ToString("HH:mm"),
				CloseTime = facility.CloseTime.ToString("HH:mm"),
				Description = facility.Description,
				Type = facility.Type
			};
		}

		public async Task<bool> DeleteFacilityAsync(int id)
		{
			var facility = await _facilityRepo.GetByConditionAsync(f => f.FacilityId == id, include: f => f.Images);

			if (facility == null)
				return false;
            var now = DateTime.UtcNow.Date;
            var anyMemberships = await _membershipRepo.ExistsAsync(m => m.FacilityId == id);
            if (anyMemberships)
                throw new InvalidOperationException("Cannot delete facility with existing memberships.");



            var imagesToDelete = facility.Images.ToList();

			foreach (var image in imagesToDelete)
			{
				await _imageRepo.DeleteAsync(image);
			}

			await _facilityRepo.DeleteAsync(facility);
			await _facilityRepo.SaveAsync();
			return true;
		}

		public async Task<IEnumerable<FacilityDto>> GetFacilitiesAsync()
		{
			var facilities = await _facilityRepo.GetAllAsync();

			return facilities.Select(f => new FacilityDto
			{
				Id = f.FacilityId,
				Name = f.Name,
				LocationDesc = f.LocationDesc,
				PhoneNumber = f.PhoneNumber,
				Type = f.Type,
				IsAvailable = f.IsAvailable,
				LocationMap = f.LocationMap,
				PricePerMonth = f.PricePerMonth,
				Description = f.Description,
				OpenTime = f.OpenTime.ToString("HH:mm"),
				CloseTime = f.CloseTime.ToString("HH:mm"),
			}).ToList();
		}

		public async Task<Facility> GetFacilityAsync(int id)
		{
			return await _facilityRepo.GetAsync(id) ?? throw new KeyNotFoundException("Facility not found");
		}
		public async Task<FacilityDetailsDto> GetFacilityDetailsAsync(int id)
		{
			var facility = await _facilityRepo.GetByConditionIncludeAsync(
				f => f.FacilityId == id,
				include: q => q
					.Include(f => f.SportFacilities)
						.ThenInclude(sf => sf.Sport)
			);

			if (facility == null)
				throw new KeyNotFoundException("Facility not found");

			return new FacilityDetailsDto
			{
				FacilityId = facility.FacilityId,
				Name = facility.Name,
				LocationDesc = facility.LocationDesc,
				LocationMap = facility.LocationMap,
				PhoneNumber = facility.PhoneNumber,
				PricePerMonth = facility.PricePerMonth,
				IsAvailable = facility.IsAvailable,
				OpenTime = facility.OpenTime.ToString("HH:mm"),
				CloseTime = facility.CloseTime.ToString("HH:mm"),
				Description = facility.Description,
				Type = facility.Type,
				AverageRating = facility.AverageRating,
				TotalReviews = facility.TotalReviews,
				Sports = facility.SportFacilities.Select(sf => new SportBasicDto
				{
					Id = sf.Sport.SportId,
					Name = sf.Sport.Name
				}).ToList()
			};
		}

		public async Task<Facility> GetFacilityWithImagesAsync(int facilityId)
		{
			return await _facilityRepo.GetByConditionIncludeAsync(
				f => f.FacilityId == facilityId,
				q => q.Include(f => f.Images)
			);
		}


		public async Task UpdateFacilityAsync(Facility facility)
		{
			var oldFacility = await _facilityRepo.GetAsync(facility.FacilityId);

			await _facilityRepo.UpdateAsync(oldFacility);
			await _facilityRepo.SaveAsync();
		}

		public async Task<FacilityDto> UpdateFacilityDtoAsync(int id, UpdateFacilityDto dto)
		{
			var facility = await _facilityRepo.GetAsync(id);
			if (facility == null)
				throw new KeyNotFoundException("facility not found");

			facility.Name = dto.Name;
			facility.LocationDesc = dto.LocationDesc;
			facility.LocationMap = dto.LocationMap;
			facility.PricePerMonth = dto.PricePerMonth;
			facility.Type = dto.Type;
			facility.PhoneNumber = dto.PhoneNumber;
			facility.IsAvailable = dto.IsAvailable;
			facility.OpenTime = TimeOnly.Parse(dto.OpenTime);
			facility.CloseTime = TimeOnly.Parse(dto.CloseTime);

			await _facilityRepo.UpdateAsync(facility);
			if (dto.SportIds is not null)
			{
				// Clear existing
				var oldSports = _sportFacilityRepo.GetListByConditionAsync(sf => sf.FacilityId == id);
				foreach (var old in oldSports.Result)
					await _sportFacilityRepo.DeleteAsync(old);

				// Add new ones
				foreach (var sportId in dto.SportIds)
				{
					var sportFacility = new SportFacility
					{
						FacilityId = id,
						SportId = sportId
					};
					await _sportFacilityRepo.AddAsync(sportFacility);
				}
			}

			await _facilityRepo.SaveAsync();
			return new FacilityDto
			{
				Id = facility.FacilityId,
				Name = facility.Name,
				LocationDesc = facility.LocationDesc,
				LocationMap = facility.LocationMap,
				PhoneNumber = facility.PhoneNumber,
				PricePerMonth = facility.PricePerMonth,
				IsAvailable = facility.IsAvailable,
				OpenTime = facility.OpenTime.ToString("HH:mm"),
				CloseTime = facility.CloseTime.ToString("HH:mm"),
				Description = facility.Description,
				Type = facility.Type
			};
		}

		public async Task<List<FacilityBySportDto>> GetFacilitiesBySportAsync(int sportId)
		{
			var facilities = await _facilityRepo.GetListByConditionIncludeAsync(
				f => f.SportFacilities.Any(sf => sf.SportId == sportId),
				include: q => q
					.Include(f => f.SportFacilities)
						.ThenInclude(sf => sf.Sport)
					.Include(f => f.Images)
			);

			return facilities.Select(f => new FacilityBySportDto
			{
				Id = f.FacilityId,
				Name = f.Name,
				Description = f.Description,
				Type = (int)f.Type,
				Sports = f.SportFacilities.Select(sf => new SportBasicDto
				{
					Id = sf.Sport.SportId,
					Name = sf.Sport.Name
				}).ToList(),
				Images = f.Images.Select(img => img.Base64Data).ToList()
			}).ToList();
		}

        public async Task<bool> SetUnavailableFacilityAsync(int facilityId)
        {
            var facility = await _facilityRepo.GetAsync(facilityId);
            if (facility == null)
                return false;

            // Check for active memberships
            var now = DateTime.UtcNow.Date;
            var activeMemberships = await _membershipRepo.GetListByConditionAsync(
                m => m.FacilityId == facilityId && m.ExpirationDate >= now && m.IsActive
            );

            if (activeMemberships.Any())
                throw new InvalidOperationException("Cannot archive facility with active memberships.");

            facility.IsAvailable = false;
            await _facilityRepo.UpdateAsync(facility);
            await _facilityRepo.SaveAsync();

            return true;
        }

        public async Task<bool> ReactivateFacilityAsync(int facilityId)
        {
            var facility = await _facilityRepo.GetAsync(facilityId);
            if (facility == null) return false;

            facility.IsAvailable = true;
            await _facilityRepo.UpdateAsync(facility);
            await _facilityRepo.SaveAsync();
            return true;
        }

        public async Task<List<FacilityBySportDto>> GetPagFacilitiesBySportAsync(int sportId, PaginationDto pagination)
        {
            var facilities = await _facilityRepo.GetListByConditionIncludeAsync(
                f => f.SportFacilities.Any(sf => sf.SportId == sportId),
                include: q => q
                    .Include(f => f.SportFacilities)
                        .ThenInclude(sf => sf.Sport)
                    .Include(f => f.Images)
            );

            var paged = facilities
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            return paged.Select(f => new FacilityBySportDto
            {
                Id = f.FacilityId,
                Name = f.Name,
                Description = f.Description,
                Type = (int)f.Type,
                Sports = f.SportFacilities.Select(sf => new SportBasicDto
                {
                    Id = sf.Sport.SportId,
                    Name = sf.Sport.Name
                }).ToList(),
                Images = f.Images.Select(img => img.Base64Data).ToList()
            }).ToList();
        }
    }
}
