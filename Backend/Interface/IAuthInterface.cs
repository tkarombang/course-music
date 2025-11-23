using Backend.DTOs.Auth;
using Backend.DTOs.ForgotPassword;
using Backend.Models;

namespace Backend.Interface
{
  public interface IAuthInterface
  {
    Task<UserModel> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<string> ForgotPasswordAsync(ForgotPasswordDto dto);
  }

}