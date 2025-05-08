using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DataAccess.Configurartions
{
	public class ReviewConfig : IEntityTypeConfiguration<Review>
	{
		public void Configure(EntityTypeBuilder<Review> builder)
		{
			// Ensure only one of FacilityId or FieldId is set
			builder.HasCheckConstraint("CK_Review_SingleTarget",
				"(FacilityId IS NOT NULL AND FieldId IS NULL) OR (FacilityId IS NULL AND FieldId IS NOT NULL)");

			// User can only review once per Facility/Field
			builder.HasIndex(r => new { r.UserId, r.FacilityId, r.FieldId }).IsUnique();


			builder.HasOne(r => r.User)
				.WithMany()  // Note: No navigation property back to reviews in User
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(r => r.Facility)
				.WithMany()
				.HasForeignKey(r => r.FacilityId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(r => r.Field)
				.WithMany()
				.HasForeignKey(r => r.FieldId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
