using Backend.DTOs.Auth;
using Backend.DTOs.ForgotPassword;
using Backend.Models;

namespace Backend.Interface
{
  public interface IAuthInterface
  {
    Task<UserModel> RegisterAsync(RegisterDto dto);
    Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginDto dto, string? ipAddress);
    Task<string> ForgotPasswordAsync(ForgotPasswordDto dto);
    // Task<string> RefreshTokenAsync(string refreshToken, string ipAddress);
    // Task<bool> RevokeRefreshTokenAsync(string refreshToken);
  }

}