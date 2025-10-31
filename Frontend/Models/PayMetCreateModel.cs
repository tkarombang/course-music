using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;

namespace Frontend.Models
{
  public class PayMetCreateModel
  {
    public int Id { get; set; }

    [JsonPropertyName("name_Methode")]
    public string? NameMethod { get; set; }

    [JsonPropertyName("logo_Methode")]
    public IBrowserFile LogoMethod { get; set; }

    public string? Status { get; set; }

    public string? CurrentLogoUrl { get; set; }
  }

}