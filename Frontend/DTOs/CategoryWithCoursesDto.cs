using System.Collections.Generic;

namespace Frontend.DTOs
{
  public class CategoryWithCoursesDto
  {
    public string NameCategory { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
    public string? ImageBanner { get; set; }
    public List<CourseDto>? Courses { get; set; }
  }
}