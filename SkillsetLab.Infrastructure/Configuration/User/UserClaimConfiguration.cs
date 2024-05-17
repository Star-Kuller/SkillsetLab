using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkillsetLab.Infrastructure.Configuration.User
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTable("user_claims");
            
            builder.HasOne(e => e.User)
                .WithMany(e => e.Claims)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}
