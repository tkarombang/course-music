namespace Backend.DTOs.Payment
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdPaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime Created_At { get; set; }
        public string PaymentMethodName { get; set; }
    }
}
