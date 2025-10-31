using Backend.DTOs.Category;
namespace Backend.DTOs.Course
{
    public class CourseDto
    {
        public int IdCourse { get; set; }
        public string NamaCourse { get; set; }
        public decimal Harga { get; set; }
        public string ImageCourse { get; set; }
        public string? Deskripsi { get; set; }
        public int IdCategory { get; set; }

        // Untuk menampilkan relasi Category juga
        public CategoryDto Category { get; set; }
    }
}
