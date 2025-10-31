namespace Admin.Models
{
    public class CourseModel
    {
        public int IdCourse { get; set; }
        public string NamaCourse { get; set; } = string.Empty;
        public decimal Harga { get; set; }
        public string? Deskripsi { get; set; }
        public string ImageCourse { get; set; } = string.Empty;

        public int IdCategory { get; set; }
    }
}
