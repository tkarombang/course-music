using System.Text.Json.Serialization;

namespace Admin.Models
{
  public class UserManModel
  {
    public int Id {get;set;}
    
    [JsonPropertyName("username")]
    public string? UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password {get;set;} = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
  }
}