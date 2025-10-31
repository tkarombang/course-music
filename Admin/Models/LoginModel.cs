using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
  public class LoginModel
  {
    [Required(ErrorMessage = "Email harus diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    public string Email { get; set; } = string.Empty;


    [Required(ErrorMessage = "Password harus diisi")]
    public string Password { get; set; } = string.Empty;
  }

  public class LoginResponse
  {
    public string Token { get; set; } = string.Empty;
  }
}