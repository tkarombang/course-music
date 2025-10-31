namespace Backend.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public int IdUser { get; set; }
        public decimal Amount { get; set; }          // ✅ Tambahkan
        public int IdPaymentMethod { get; set; }
        public string Status { get; set; }           // ✅ Tambahkan
    }
}