namespace Backend.DTOs.Checkout
{
    public class CheckoutDto
    {
        public int IdCheckout { get; set; }
        public int IdUser { get; set; }
        public int IdCourse { get; set; }
        public string ImageCourse { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public DateTime Jadwal { get; set; }

        public string NamaCourse { get; set; }
        public decimal Harga { get; set; }
    }
}
