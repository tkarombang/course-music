using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

public class ForgotPasswordModels
{
  [Required(ErrorMessage = "EMAIL HARUS DIISI")]
  [EmailAddress(ErrorMessage = "FORMAT EMAIL TIDAK VALID")]
  public string? Email { get; set; }
}
public class NewPasswordModels
{
  [Required(ErrorMessage = "PASSWORD BARU HARUS DIISI")]
  [MinLength(8, ErrorMessage = "PASSOWRD MINIMAL 8 KARAKTER")]
  public string? NewPassword { get; set; }


  [Required(ErrorMessage = "KONFIRMASI PASSWORD HARUS DIISI")]
  [Compare(nameof(NewPassword), ErrorMessage = "PASSWORD TIDAK COCOK")]
  public string? ConfirmPassword { get; set; }
}