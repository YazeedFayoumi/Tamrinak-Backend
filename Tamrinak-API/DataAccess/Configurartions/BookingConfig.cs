using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DataAccess.Configurartions
{
	public class BookingConfig : IEntityTypeConfiguration<Booking>
	{
		public void Configure(EntityTypeBuilder<Booking> builder)
		{
			builder.HasOne(b => b.User)
				.WithMany(u => u.Bookings)
				.HasForeignKey(b => b.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(b => b.Field)
				.WithMany(f => f.Bookings)
				.HasForeignKey(b => b.FieldId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(b => b.SportFacility)
				.WithMany(sf => sf.Bookings)
				.HasForeignKey(b => new { b.SportId, b.FacilityId }).
				OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(b => b.Payment)
				.WithOne(p => p.Booking)
				.HasForeignKey<Payment>(p => p.BookingId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
