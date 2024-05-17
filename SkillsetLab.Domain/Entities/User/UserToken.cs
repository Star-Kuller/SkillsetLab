using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User;

public class UserToken : IdentityUserToken<long>
{
    public virtual User User { get; set; }
}