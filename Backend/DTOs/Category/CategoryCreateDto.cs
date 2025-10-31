using Microsoft.AspNetCore.Http;

public class CategoryCreateDto
{
    public string NameCategory { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
    public IFormFile? ImageCategory { get; set; }
    public IFormFile? ImageBanner { get; set; }
}
