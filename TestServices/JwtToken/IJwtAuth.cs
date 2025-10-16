using System.Security.Claims;
using Testdata.Viewmodel;

namespace TestServices.JwtToken
{
    public interface IJwtAuth
    {
        string GenerateAccessToken(Logindata model);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
