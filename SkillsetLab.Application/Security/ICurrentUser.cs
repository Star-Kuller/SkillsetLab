namespace Application.Security;

public interface ICurrentUser
{
    long Id { get; set; }
    string Email { get; set; }
    string Role { get; set; }
}