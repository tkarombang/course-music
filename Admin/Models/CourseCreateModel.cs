using Microsoft.AspNetCore.Components.Forms;

namespace Admin.Models
{
    public class CourseCreateModel
    {
        public int? IdCourse { get; set; }
        public string NamaCourse { get; set; } = string.Empty;
        public int IdCategory { get; set; }
        public decimal Harga { get; set; }
        public IBrowserFile ImageCourse { get; set; }
        public string? Deskripsi { get; set; }
        public string? CurrentImageUrl { get; set; }
        public string? ImageBase64 { get; set; }       
        public string? ImageFileName { get; set; }
    }

}