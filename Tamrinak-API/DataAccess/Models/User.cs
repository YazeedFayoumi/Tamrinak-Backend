﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
	public class User
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }

		[Required, MaxLength(50)]
		public string Name { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required]
		public string PasswordHash { get; set; }

		public bool IsActive { get; set; } = true;
		public bool IsEmailConfirmed { get; set; } = false;
		[DataType(DataType.Date)]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public string? ProfileImageBase64 { get; set; }

        public bool HasVenueOwnershipRequest { get; set; } = false;
        public DateTime? VenueRequestDate { get; set; }

        public int? RequestedVenueId { get; set; } // ID of the field or facility
        public string? RequestedVenueType { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
		public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
		public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

	}
}
