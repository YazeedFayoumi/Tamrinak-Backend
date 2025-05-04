using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using Tamrinak_API.DataAccess.Configurartions;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FacilityDtos;
using Tamrinak_API.DTO.FieldDtos;
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
        public FacilityService(IGenericRepo<Facility> facilityRepo,IGenericRepo<SportFacility> sportFacilityRepo, IGenericRepo<Image> imageRepo, IGenericRepo<Sport> sportRepo)
        {
            _facilityRepo = facilityRepo;
            _sportFacilityRepo = sportFacilityRepo;
            _imageRepo = imageRepo;
            _sportRepo = sportRepo;
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
                OfferDurationInMonths = dto.OfferDurationInMonths,
                OfferPrice = dto.OfferPrice,
                Description = dto.Description,
                OpenTime = dto.OpenTime,
                CloseTime = dto.CloseTime
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
                OfferDurationInMonths = facility.OfferDurationInMonths,
                OfferPrice = facility.OfferPrice,
                IsAvailable = facility.IsAvailable,
                OpenTime = facility.OpenTime,
                CloseTime = facility.CloseTime,
                Description = facility.Description,
                Type = facility.Type
            };
        }

        public async Task<bool> DeleteFacilityAsync(int id)
        {
            var facility = await _facilityRepo.GetByConditionAsync(f => f.FacilityId == id, include: f => f.Images);

            if (facility == null)
                return false;

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
                OfferDurationInMonths = f.OfferDurationInMonths,
                OfferPrice = f.OfferPrice,
                Description = f.Description,
                OpenTime = f.OpenTime,
                CloseTime = f.CloseTime,
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
                OfferDurationInMonths = facility.OfferDurationInMonths,
                OfferPrice = facility.OfferPrice,
                IsAvailable = facility.IsAvailable,
                OpenTime = facility.OpenTime,
                CloseTime = facility.CloseTime,
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
            facility.OfferDurationInMonths = dto.OfferDurationInMonths;
            facility.OfferPrice = dto.OfferPrice;
            facility.Type = dto.Type;
            facility.PhoneNumber = dto.PhoneNumber;
            facility.IsAvailable = dto.IsAvailable;
            facility.OpenTime = dto.OpenTime;
            facility.CloseTime = dto.CloseTime;

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
                OfferDurationInMonths = facility.OfferDurationInMonths,
                OfferPrice = facility.OfferPrice,
                IsAvailable = facility.IsAvailable,
                OpenTime = facility.OpenTime,
                CloseTime = facility.CloseTime,
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
                Images = f.Images.Select(img => img.Url).ToList()
            }).ToList();
        }
    }
}
