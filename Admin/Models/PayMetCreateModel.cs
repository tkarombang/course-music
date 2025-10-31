using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;

namespace Admin.Models
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
        public string? LogoBase64 { get; set; }
        public string? LogoFileName { get; set; }
    }

}