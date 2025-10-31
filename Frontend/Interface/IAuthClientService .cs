using System.IdentityModel.Tokens.Jwt;
using Frontend.DTOs;
using Frontend.Models;

namespace Frontend.Interface
{
  public interface IAuthClientService
  {
    Task<string> Login(LoginRequestDto dto);
    Task<bool> Register(RegisterRequestDto dto);

    Task<bool> ForgotPassword(ForgotPasswordRequestDto dto);
    Task<bool> ResetPassword(ResetPasswordRequestDto dto);
    
    Task Logout();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsTokenValidAsync();
    
    Task CheckAndRedirectAsync();

    Task<int?> GetUserIdAsync();
    
    Task<JwtSecurityToken?> ParseJwtTokenAsync();


  }
}