using Application.Interfaces;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillsetLab.Infrastructure.Extentions;

namespace SkillsetLab.Infrastructure;

public class MyDbContext : IdentityDbContext<
    User, Role, long,
    UserClaim, UserRole, UserLogin,
    RoleClaim, UserToken>, IMyDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
        
        modelBuilder.UseSerialColumns();
        modelBuilder.Seed();
    }
}