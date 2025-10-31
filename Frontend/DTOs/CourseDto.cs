namespace Frontend.DTOs
{
  public class CourseDto
  {
    public int IdCourse { get; set; }
    public string NamaCourse { get; set; } = string.Empty;
    public string ImageCourse { get; set; } = string.Empty;
    public decimal Harga { get; set; }
    public string Deskripsi { get; set; } = string.Empty;
  }
}
