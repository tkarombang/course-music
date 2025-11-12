namespace Backend.DTOs.Course
{
    public class CreateCourseDto
    {
        public string NamaCourse { get; set; } = string.Empty;
        public decimal Harga { get; set; }
        public string? Deskripsi { get; set; }
        public IFormFile? ImageCourse { get; set; }
        public IFormFile? ImageBanner { get; set; }
        public int IdCategory { get; set; }
    }
}
