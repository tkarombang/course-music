using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class PaymentMethodModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name_Methode { get; set; }

        public string Logo_Methode { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        // Navigation
        public ICollection<PaymentModel> Payments { get; set; }
    }
}
