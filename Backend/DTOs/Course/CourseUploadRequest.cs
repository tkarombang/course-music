using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.Course
{
    public class CourseUploadRequest
    {
        [Required]
        public string NamaCourse { get; set; } = string.Empty;

        [Required]
        public decimal Harga { get; set; }

        public string? Deskripsi { get; set; }

        [Required]
        public int IdCategory { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
