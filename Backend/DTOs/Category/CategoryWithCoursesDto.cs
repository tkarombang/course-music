namespace Backend.DTOs.Course
{
    public class CategoryWithCoursesDto
    {
        public int IdCategory { get; set; }
        public string NameCategory { get; set; } = string.Empty;
        public string? ImageCategory { get; set; }
        public string? Deskripsi { get; set; }
        public string? ImageBanner { get; set; }
        public List<CourseInCategoryDto>? Courses { get; set; }
    }
}
