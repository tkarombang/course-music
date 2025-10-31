using System.ComponentModel.DataAnnotations;
namespace Backend.DTOs.Auth
{
  public class RegisterDto
  {
    [Required]
    public string Namalengkap { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;


    public string Role { get; set; } = "User";
  }
}