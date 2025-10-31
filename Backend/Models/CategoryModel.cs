using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class CategoryModel
    {
        [Key] // Primary Key
        public int IdCategory { get; set; }

        [Required]
        [MaxLength(100)] // varchar(100)
        public string NameCategory { get; set; } = string.Empty;

        public string? ImageCategory { get; set; }
        public string? ImageBanner { get; set; }

        [MaxLength(1500)] 
        public string? Deskripsi { get; set; } 

        // Relasi 1 Category bisa punya banyak Course
        public ICollection<DataCourseModel>? Courses { get; set; }
    }
}
