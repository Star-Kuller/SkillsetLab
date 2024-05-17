using Domain.Entities.Comon;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User;

public class User : IdentityUser<long>
{
    public Status Status { get; set; }
    public virtual IList<UserClaim> Claims { get; set; }
    public virtual IList<UserLogin> Logins { get; set; }
    public virtual IList<UserToken> Tokens { get; set; }
    public virtual IList<UserRole> UserRoles { get; set; }
    
    public User()
    {
        Claims = new List<UserClaim>();
        Logins = new List<UserLogin>();
        Tokens = new List<UserToken>();
        UserRoles = new List<UserRole>();
    }
}