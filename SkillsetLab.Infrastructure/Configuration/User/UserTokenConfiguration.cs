using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkillsetLab.Infrastructure.Configuration.User
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable("user_tokens");
            
            builder.HasOne(e => e.User)
                .WithMany(e => e.Tokens)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}
