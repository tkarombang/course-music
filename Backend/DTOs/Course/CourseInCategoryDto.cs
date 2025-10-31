public class CourseInCategoryDto
    {
        public int IdCourse { get; set; }
        public string NamaCourse { get; set; } = string.Empty;
        public decimal Harga { get; set; }
        public string? ImageCourse { get; set; }
        public string? Deskripsi { get; set; }
    }