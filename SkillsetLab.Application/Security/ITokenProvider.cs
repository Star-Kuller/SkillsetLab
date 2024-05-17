namespace Application.Security;

public interface ITokenProvider
{
    string GetToken(UserClaims userClaims);
}