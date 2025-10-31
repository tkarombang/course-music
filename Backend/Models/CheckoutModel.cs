using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class CheckoutModel
    {
        [Key]
        public int IdCheckout { get; set; }

        [ForeignKey("User")]
        public int IdUser { get; set; }

        [ForeignKey("Course")]
        public int IdCourse { get; set; }

        [ForeignKey("Payment")]
        public int? IdPayment { get; set; }

        public DateTime Jadwal { get; set; }

        // Tambahkan properti IsPaid di CheckoutModel
        public bool IsPaid { get; set; } = false;


        // Navigation
        public UserModel User { get; set; }
        public DataCourseModel Course { get; set; }
        public PaymentModel Payment { get; set; }
    }
}
