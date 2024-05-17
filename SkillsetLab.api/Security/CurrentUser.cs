using Application.Security;

namespace SkillsetLab.Security
{
    public class CurrentUser : ICurrentUser
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
