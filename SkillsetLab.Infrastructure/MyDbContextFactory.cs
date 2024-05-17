using Microsoft.EntityFrameworkCore;
using SkillsetLab.Infrastructure.Infrastructure;

namespace SkillsetLab.Infrastructure;

public class MyDbContextFactory : DesignTimeDbContextFactoryBase<MyDbContext>
{
    protected override MyDbContext CreateNewInstance(DbContextOptions<MyDbContext> options)
    {
        return new MyDbContext(options);
    }
}