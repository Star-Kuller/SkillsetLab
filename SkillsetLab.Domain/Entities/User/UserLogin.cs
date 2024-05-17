using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User;

public class UserLogin : IdentityUserLogin<long>
{
    public virtual User User { get; set; }
}