using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Frontend.DTOs
{
  public class ForgotPasswordRequestDto
  {
    [JsonPropertyName("email")]
    [Required(ErrorMessage = "EMAIL HARUS DIISI")]
    [EmailAddress(ErrorMessage = "FORMAT EMAIL TIDAK VALID")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("newPassword")]
    [Required(ErrorMessage = "PASS BARU WAJIB DISIS")]
    [MinLength(6, ErrorMessage = "PASSWORD MINIMAL 6 KARAKTER.")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare(nameof(NewPassword), ErrorMessage = "KONFIRMASI PASSWORD TIDAK SAMA.")]
    [Required(ErrorMessage = "KONFIRMASI PASSWORD WAJIB DIISI")]
    public string ConfirmPassword { get; set; } = string.Empty;
  }

}
