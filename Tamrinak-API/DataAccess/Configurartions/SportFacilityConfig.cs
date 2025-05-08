using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DataAccess.Configurartions
{
	public class SportFacilityConfig : IEntityTypeConfiguration<SportFacility>
	{
		public void Configure(EntityTypeBuilder<SportFacility> builder)
		{
			builder.HasKey(fs => new { fs.FacilityId, fs.SportId });

			builder.HasOne(fs => fs.Facility)
				.WithMany(f => f.SportFacilities)
				.HasForeignKey(fs => fs.FacilityId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(fs => fs.Sport)
				.WithMany(s => s.SportFacilities)
				.HasForeignKey(fs => fs.SportId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
