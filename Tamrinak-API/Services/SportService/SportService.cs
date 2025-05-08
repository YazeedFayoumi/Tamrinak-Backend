using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.SportDtos;
using Tamrinak_API.DTO.UserAuthDtos;
using Tamrinak_API.Repository.GenericRepo;
using Tamrinak_API.Services.ImageService;

namespace Tamrinak_API.Services.SportService
{
	public class SportService : ISportService
	{
		private readonly IGenericRepo<Sport> _sportRepo;
		private readonly IImageService _imageService;

		public SportService(IGenericRepo<Sport> sportRepo, IImageService imageService)
		{
			_sportRepo = sportRepo;
			_imageService = imageService;
		}

		public async Task<SportDto> AddSportAsync(AddSportDto dto)
		{
			var sport = new Sport
			{
				Name = dto.Name,
				Description = dto.Description
			};

			var newSport = await _sportRepo.CreateAsync(sport);
			await _sportRepo.SaveAsync();
			return new SportDto
			{
				Id = newSport.SportId,
				Name = newSport.Name,
				Description = newSport.Description
			};


		}

		public async Task<IEnumerable<SportDto>> GetAllSportsAsync()
		{
			/*var sports = await _sportRepo.GetAllAsync();
            return sports.Select(s => new SportDto
            {
                Id = s.SportId,
                Name = s.Name,
                Description = s.Description
                
            }).ToList();*/

			var sports = await _sportRepo.GetListByConditionIncludeAsync(predicate: s => true,
				include: q => q.Include(s => s.Images));


			return sports.Select(s => new SportDto
			{
				Id = s.SportId,
				Name = s.Name,
				IconUrl = s.Images.FirstOrDefault()?.Url // ⬅️ fetch sport icon
			}).ToList();


		}

		public async Task<Sport> GetSportByIdAsync(int id)
		{
			return await _sportRepo.GetAsync(id);
		}

		public async Task UpdateSportDtoAsync(int id, UpdateSportDto dto)
		{
			var sport = await _sportRepo.GetAsync(id);
			if (sport == null)
				throw new KeyNotFoundException("Sport not found");

			sport.Name = dto.Name;
			sport.Description = dto.Description;

			await _sportRepo.UpdateAsync(sport);

		}
		public async Task UpdateSportAsync(Sport sport)
		{

			await _sportRepo.UpdateAsync(sport);
			await _sportRepo.SaveAsync();

		}


		public async Task DeleteSportAsync(int id)
		{
			var sport = await _sportRepo.GetByConditionIncludeAsync(
				s => s.SportId == id,
				include: q => q.Include(s => s.Images)
			);

			if (sport == null)
				throw new KeyNotFoundException("Sport not found");

			// Delete associated images first
			foreach (var image in sport.Images.ToList())
			{
				await _imageService.DeleteImageAsync(image.Url); // make sure this removes from DB and storage if needed
			}
			await _sportRepo.DeleteAsync(sport);
			await _sportRepo.SaveAsync();
		}

		public async Task<SportDetailsDto> GetSportDetailAsync(int id)
		{
			var sport = await _sportRepo.GetAsync(id);
			if (sport == null) throw new KeyNotFoundException("Not Found");
			return new SportDetailsDto
			{
				Id = sport.SportId,
				Name = sport.Name,
				Description = sport.Description,
				ImageUrls = sport.Images.Select(u => u.Url).ToList(),
			};
		}
	}
}
