using Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.Repository.GenericRepo;

namespace Tamrinak_API.DataAccess.Seeding
{
	public class DatabaseInitializer
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<DatabaseInitializer> _logger;
		private readonly IGenericRepo<User> _userRepo;
		private readonly IGenericRepo<UserRole> _roleRepo;

		public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger, IGenericRepo<User> userRepo, IGenericRepo<UserRole> roleRepo)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
			_roleRepo = roleRepo;
			_userRepo = userRepo;
		}

		public async Task InitializeAsync()
		{
			using var scope = _serviceProvider.CreateScope();
			var services = scope.ServiceProvider;

			try
			{
				var dbContext = services.GetRequiredService<DatabaseContext>();
				if (dbContext.Database.CanConnect())
				{
					await dbContext.Database.MigrateAsync();

					await SeedAdminUserAsync(services);
				}
				else
				{
					_logger.LogWarning("Database is not accessible.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while initializing the database.");
			}
		}

		private async Task SeedAdminUserAsync(IServiceProvider services)
		{
			var email = "Admin@adm.com";
			var password = BCrypt.Net.BCrypt.HashPassword("Admin1234");

			User user = await _userRepo.GetByConditionAsync(e => e.Email == email);

			if (user == null)
			{
				var adminUser = new User
				{
					Name = "Admin",
					Email = email,
					IsEmailConfirmed = true,
					CreatedAt = DateTime.UtcNow,
					IsActive = true,
					PasswordHash = password,
				};

				await _userRepo.AddAsync(adminUser);
				var userRole = new UserRole
				{
					UserId = adminUser.UserId,
					RoleId = 1
				};
				await _roleRepo.AddAsync(userRole);
			}
		}
	}
}

