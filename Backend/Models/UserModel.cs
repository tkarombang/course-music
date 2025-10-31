using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(20)]
        public string Role { get; set; } = "User"; // default user

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? PasswordResetToken { get; set; }
        public DateTime? ResetPasswordExpiry { get; set; }

        // Navigation
        public ICollection<CheckoutModel> Checkouts { get; set; }
        public ICollection<PaymentModel> Payments { get; set; }
        public DateTime ResetTokenExpiry { get; internal set; }
    }

}
