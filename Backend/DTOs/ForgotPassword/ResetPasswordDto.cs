using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.ForgotPassword
{
  public class ResetPasswordDto
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
    public string NewPassword { get; set; } = string.Empty;

  }
}