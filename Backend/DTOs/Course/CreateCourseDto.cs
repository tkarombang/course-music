namespace Backend.DTOs.Course
{
    public class CreateCourseDto
    {
        public string NamaCourse { get; set; } = string.Empty;
        public decimal Harga { get; set; }
        public string? Deskripsi { get; set; }
        public string? ImageCourse { get; set; }
        public int IdCategory { get; set; }
    }
}
