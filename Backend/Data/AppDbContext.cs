using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DataCourseModel> Courses { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }

        // New tables
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CheckoutModel> Checkouts { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<PaymentMethodModel> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relasi Course.IdCategory → Category.IdCategory
            modelBuilder.Entity<DataCourseModel>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Courses)
                .HasForeignKey(c => c.IdCategory)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment → User
            modelBuilder.Entity<PaymentModel>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.IdUser)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment → PaymentMethod
            modelBuilder.Entity<PaymentModel>()
                .HasOne(p => p.PaymentMethod)
                .WithMany()
                .HasForeignKey(p => p.IdPaymentMethod)
                .OnDelete(DeleteBehavior.Restrict);

            // Checkout → User
            modelBuilder.Entity<CheckoutModel>()
                .HasOne(c => c.User)
                .WithMany(u => u.Checkouts)
                .HasForeignKey(c => c.IdUser)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentModel>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            // 🔹 Relasi 1 Payment -> banyak Checkout
            modelBuilder.Entity<CheckoutModel>()
                .HasOne(c => c.Payment)
                .WithMany(p => p.Checkouts)
                .HasForeignKey(c => c.IdPayment)
                .OnDelete(DeleteBehavior.SetNull); // jika payment dihapus, IdPayment jadi null
        }

    }
}
