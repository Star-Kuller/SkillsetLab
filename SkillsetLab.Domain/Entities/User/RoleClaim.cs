using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User;

public class RoleClaim : IdentityRoleClaim<long>
{
    public virtual Role Role { get; set; }
}