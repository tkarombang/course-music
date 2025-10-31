using Backend.DTOs.Auth;
using Backend.Models;

public interface IAuthService
{
  Task<UserModel> RegisterAsync(RegisterDto dto);
  Task<string> LoginAsync(LoginDto dto);
}