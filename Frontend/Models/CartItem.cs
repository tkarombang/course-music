using System.ComponentModel.DataAnnotations;

namespace Cart.Models
{
    public class CartItem
    {
        // Id course sesuai database/backend
        public int IdCourse { get; set; }

        public string ClassName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public bool IsSelected { get; set; } = false;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime Jadwal { get; set; }

        // Local identifier untuk operasi CRUD di UI (hapus, dll)
        public Guid LocalId { get; set; } = Guid.NewGuid();
    }
}

