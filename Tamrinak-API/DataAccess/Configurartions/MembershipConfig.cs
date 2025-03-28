using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DataAccess.Configurartions
{
    public class MembershipConfig : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasOne(m => m.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Facility)
                .WithMany(f => f.Memberships)
                .HasForeignKey(m => m.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Payment)
               .WithOne(p => p.Membership)
               .HasForeignKey<Payment>(p => p.MembershipId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
