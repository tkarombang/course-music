using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class DataCourseModel
    {
        [Key] // Primary Key
        public int IdCourse { get; set; }

        [Required]
        [MaxLength(100)] // varchar(100)
        public string NamaCourse { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")] // Decimal
        public decimal Harga { get; set; }

        public string? ImageCourse { get; set; }

        [MaxLength(1500)] 
        public string? Deskripsi { get; set; } 

        // Foreign Key
        public int IdCategory { get; set; }

        // Navigation property untuk relasi
        public CategoryModel? Category { get; set; }
    }
}
