using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DataAccess.Seeding
{
	public static class RoleSeeder
	{
		public static void SeedRoles(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Role>().HasData(

				new Role
				{
					RoleId = 1,
					RoleName = "SuperAdmin",
					Description = "Full system access and control"
				},
				new Role
				{
					RoleId = 2,
					RoleName = "Admin",
					Description = "Administrative access with most system privileges"
				},

				new Role
				{
					RoleId = 3,
					RoleName = "User",
					Description = "Standard registered user"
				},


				new Role
				{
					RoleId = 4,
					RoleName = "VenueManager",
					Description = "Manages specific venue (facility and field) operations"
				},


				new Role
				{
					RoleId = 7,
					RoleName = "CustomerSupport",
					Description = "Handles customer inquiries and support"
				},
				new Role
				{
					RoleId = 8,
					RoleName = "Moderator",
					Description = "Content and user behavior moderation"
				}


			);
		}
	}
}
