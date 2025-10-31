using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.ForgotPassword
{
  public class ForgotPasswordDto
  {
    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;
  }
}