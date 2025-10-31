using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Frontend.Models
{
  public class RegisterModel
  {
        [JsonPropertyName("namalengkap")]
        [Required(ErrorMessage = "Nama lengkap wajib diisi.")]
        public string NamaLengkap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Email tidak valid.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi.")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konfirmasi password wajib diisi.")]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi password harus sama.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}