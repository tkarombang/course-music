using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Admin.Models
{
  public class PayMetModel
  {
    public int Id { get; set; }

    [JsonPropertyName("name_Methode")]
    public string NameMethod { get; set; } = string.Empty;
    
    [JsonPropertyName("logo_Methode")]
    public string LogoMethod { get; set; } = string.Empty;
    
    public string Status { get; set; } = string.Empty;
  }
}