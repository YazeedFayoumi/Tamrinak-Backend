using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tamrinak_API.DataAccess.Models
{
	public class Booking
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int BookingId { get; set; }

		[Required]
		public int UserId { get; set; }

		public int? FieldId { get; set; }

		[Required, DataType(DataType.Date)]
		public DateTime BookingDate { get; set; }
		[Required]
		public int NumberOfPeople { get; set; }

		[Required]
		public TimeOnly StartTime { get; set; }

		[Required]
		public TimeOnly EndTime { get; set; }
		public TimeSpan Duration => (EndTime - StartTime); //EndTime.ToTimeSpan() - StartTime.ToTimeSpan();

		public decimal TotalCost { get; set; }
		public bool IsPaid { get; set; } = false;
		public BookingStatus Status { get; set; } = BookingStatus.Pending;

		public DateTime CreatedAt { get; set; }
		public DateTime? CancelledAt { get; set; }

		public User User { get; set; }
		public Field Field { get; set; }
		public Payment Payment { get; set; }
		public int? PaymentId { get; set; }
		public int? FacilityId { get; set; }
		public int? SportId { get; set; }
		public SportFacility SportFacility { get; set; }
	}
	public enum BookingStatus
	{
		Pending,
		Cancelled,
		Completed
	}
}
