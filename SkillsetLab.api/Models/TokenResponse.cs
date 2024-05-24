using System.ComponentModel;

namespace SkillsetLab.Models
{
    public class TokenResponse
    {
        [Description("JWT-токен")]
        public string AccessToken { get; set; }
        [Description("Роль")]
        public string Role { get; set; }
    }
}
