using System.Text.Json.Serialization;

namespace Frontend.DTOs
{
  public class RegisterRequestDto
  {
    [JsonPropertyName("namalengkap")]
    public string NamaLengkap { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
  }
}