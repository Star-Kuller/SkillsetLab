using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace SkillsetLab.Infrastructure.Extentions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(GetRoles());
        }

        public static Role[] GetRoles()
        {
            return new[]
            {
                new Role
                {
                    Id = 1,
                    Name = Role.Administrator,
                    NormalizedName = Role.Administrator.ToUpper(),
                    ConcurrencyStamp = "987e6543-e21b-98d3-b456-987654321125"
                },
                new Role
                {
                    Id = 2,
                    Name = Role.Standard,
                    NormalizedName = Role.Standard.ToUpper(),
                    ConcurrencyStamp = "987e6543-e21b-98d3-b456-98765432199c"
                }
            };
        }
    }
}
