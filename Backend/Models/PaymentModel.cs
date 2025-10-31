using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class PaymentModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int IdUser { get; set; }

        [ForeignKey("PaymentMethod")]
        public int IdPaymentMethod { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public UserModel User { get; set; }
        public PaymentMethodModel PaymentMethod { get; set; }

        public ICollection<CheckoutModel> Checkouts { get; set; }
    }
}
