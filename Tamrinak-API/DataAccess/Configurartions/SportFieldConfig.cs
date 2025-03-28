using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DataAccess.Configurartions
{
    public class SportFieldConfig : IEntityTypeConfiguration<SportField>
    {
        public void Configure(EntityTypeBuilder<SportField> builder)
        {
            builder.HasKey(fs => new { fs.FieldId, fs.SportId });

            builder.HasOne(fs => fs.Field)
                .WithMany(f => f.SportFields)
                .HasForeignKey(fs => fs.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fs => fs.Sport)
                .WithMany(s => s.SportFields)
                .HasForeignKey(fs => fs.SportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
