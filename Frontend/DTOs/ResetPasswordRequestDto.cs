using System.ComponentModel.DataAnnotations;

namespace Frontend.DTOs
{
  public class ResetPasswordRequestDto
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "KONFIRMASI PASSWORD HARUS DIISI")]
    [Compare(nameof(NewPassword), ErrorMessage = "PASSWORD TIDAK COCOK")]
    public string ConfirmPassword { get; set; } = string.Empty;
  }
}