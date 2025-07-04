﻿using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Configurartions;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DataAccess.Seeding;

namespace Tamrinak_API.DataAccess
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{
		}
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<Facility> Facilities { get; set; }
		public DbSet<Field> Fields { get; set; }
		public DbSet<Sport> Sports { get; set; }
		public DbSet<SportFacility> SportFacilities { get; set; }
		public DbSet<Membership> Memberships { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<Image> Images { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<MembershipOffer> MembershipsOffer { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			modelBuilder.ApplyConfiguration(new UserRoleConfig());
			modelBuilder.ApplyConfiguration(new SportFacilityConfig());
			modelBuilder.ApplyConfiguration(new ReviewConfig());
			modelBuilder.ApplyConfiguration(new BookingConfig());
			modelBuilder.ApplyConfiguration(new MembershipConfig());
			modelBuilder.ApplyConfiguration(new ImageConfig());


			RoleSeeder.SeedRoles(modelBuilder);
			SportSeeder.SeedSports(modelBuilder);
		}
	}


}

