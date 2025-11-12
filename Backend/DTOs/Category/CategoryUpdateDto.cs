using Microsoft.AspNetCore.Http;

namespace Backend.DTOs.Category
{
    public class CategoryUpdateDto
    {
        public string NameCategory { get; set; } = string.Empty;
        public string? Deskripsi { get; set; }
        public IFormFile? ImageCategory { get; set; }
        public IFormFile? ImageBanner { get; set; }
    }
}
