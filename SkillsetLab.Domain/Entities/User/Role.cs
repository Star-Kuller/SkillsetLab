using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User;

public class Role : IdentityRole<long>
{
    public const string Administrator = "Administrator";
    public const string Standard  = "Standard";
    
    public virtual IList<UserRole> UserRoles { get; set; }
    public virtual IList<RoleClaim> RoleClaims { get; set; }

    public Role()
    {
        UserRoles = new List<UserRole>();
        RoleClaims = new List<RoleClaim>();
    }
}