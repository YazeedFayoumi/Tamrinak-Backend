using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DataAccess.Configurartions
{
    public class ImageConfig : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {

            builder.HasOne(i => i.Facility)
                .WithMany(f => f.Images)
                .HasForeignKey(i => i.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(i => i.Field)
                .WithMany(f => f.Images)
                .HasForeignKey(i => i.FieldId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(i => i.Sport)
                .WithMany(s => s.Images)
                .HasForeignKey(i => i.SportId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
