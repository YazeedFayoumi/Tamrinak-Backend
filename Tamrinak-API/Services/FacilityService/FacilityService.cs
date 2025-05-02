using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.FacilityDtos;
using Tamrinak_API.DTO.FieldDtos;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.Services.FacilityService
{
    public class FacilityService : IFacilityService
    {
        private readonly IGenericRepo<Facility> _facilityRepo;
        private readonly IGenericRepo<Image> _imageRepo;
        public FacilityService(IGenericRepo<Facility> facilityRepo, IGenericRepo<Image> imageRepo) 
        {
            _facilityRepo = facilityRepo;
            _imageRepo = imageRepo;
        }

        public async Task<Facility> AddFacilityAsync(AddFacilityDto dto)
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
            return createdFac;
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

        public async Task UpdateFacilityDtoAsync(int id, FacilityDto dto)
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
            await _facilityRepo.SaveAsync();
        }
    }
}
